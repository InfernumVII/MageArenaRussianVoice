using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Dissonance;
using HarmonyLib;
using MageArenaRussianVoice.Config;
using Recognissimo;
using Recognissimo.Components;
using UnityEngine;


namespace MageArenaRussianVoice.Patches
{
    [HarmonyPatch(typeof(VoiceControlListener))]
    public static class VoiceControlListenerPatch
    {
        private static Dictionary<string[], Action<VoiceControlListener>> russianCommandMap;
        private static Dictionary<string, string[]> russianAdditionalCommandMap;
        private static readonly AccessTools.FieldRef<VoiceControlListener, SpeechRecognizer> srRef =
            AccessTools.FieldRefAccess<VoiceControlListener, SpeechRecognizer>("sr");

        private static readonly AccessTools.FieldRef<VoiceControlListener, VoiceBroadcastTrigger> vbtRef =
            AccessTools.FieldRefAccess<VoiceControlListener, VoiceBroadcastTrigger>("vbt");

        private static readonly MethodInfo restartsrMethod =
            AccessTools.Method(typeof(VoiceControlListener), "restartsr");

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void AwakePostfix(VoiceControlListener __instance)
        {
            var plugin = BepInEx.Bootstrap.Chainloader.PluginInfos.Values.FirstOrDefault(p => p.Metadata.GUID == "com.infernumvii.magearenarussianvoice");
            if (plugin != null)
            {
                VoiceCommandConfig.Init(plugin.Instance.Config);

                russianCommandMap = new Dictionary<string[], Action<VoiceControlListener>>()
                {
                    { VoiceCommandConfig.FireballCommand.Value.Split(' '), v => v.CastFireball() },
                    { VoiceCommandConfig.FrostBoltCommand.Value.Split(' '), v => v.CastFrostBolt() },
                    { VoiceCommandConfig.WormCommand.Value.Split(' '), v => v.CastWorm() },
                    { VoiceCommandConfig.HoleCommand.Value.Split(' '), v => v.CastHole() },
                    { VoiceCommandConfig.MagicMissileCommand.Value.Split(' '), v => v.CastMagicMissle() },
                    { VoiceCommandConfig.MirrorCommand.Value.Split(' '), v => v.ActivateMirror() }
                };

                russianAdditionalCommandMap = new Dictionary<string, string[]>
                {
                    {"rock", VoiceCommandConfig.RockCommand.Value.Split(' ')},
                    {"wisp", VoiceCommandConfig.WispCommand.Value.Split(' ')},
                    {"blast", VoiceCommandConfig.BlastCommand.Value.Split(' ')},
                    {"divine", VoiceCommandConfig.DivineCommand.Value.Split(' ')},
                    {"blink", VoiceCommandConfig.BlinkCommand.Value.Split(' ')},
                    {"thunderbolt", VoiceCommandConfig.ThunderboltCommand.Value.Split(' ')},
                    {"bubble", VoiceCommandConfig.BubbleCommand.Value.Split(' ')},
                    {"doom", VoiceCommandConfig.DoomCommand.Value.Split(' ')}
                };
            }
        }

        [HarmonyPatch("waitgetplayer")]
        [HarmonyPrefix]
        private static bool WaitGetPlayerPrefix(VoiceControlListener __instance, ref IEnumerator __result)
        {
            __result = ModifiedWaitGetPlayer(__instance);
            return false;
        }

        private static IEnumerator ModifiedWaitGetPlayer(VoiceControlListener instance)
        {
            while (instance.pi == null)
            {
                if (Camera.main.transform.parent != null &&
                    Camera.main.transform.parent.TryGetComponent<PlayerInventory>(out var playerInventory))
                {
                    instance.pi = playerInventory;
                }
                yield return null;
            }

            instance.GetComponent<SetUpModelProvider>().Setup();
            yield return null;

            srRef(instance) = instance.GetComponent<SpeechRecognizer>();
            var recognizer = srRef(instance);
            recognizer.LanguageModelProvider = instance.GetComponent<StreamingAssetsLanguageModelProvider>();
            recognizer.SpeechSource = instance.GetComponent<DissonanceSpeechSource>();
            instance.SpellPages = new List<ISpellCommand>();
            MonoBehaviour[] components = instance.gameObject.GetComponents<MonoBehaviour>();
            for (int i = 0; i < components.Length; i++)
            {
                ISpellCommand spellCommand = components[i] as ISpellCommand;
                if (spellCommand != null)
                {
                    instance.SpellPages.Add(spellCommand);
                }
            }
            // foreach (ISpellCommand spellCommand2 in instance.SpellPages)
            // {
            //     spellCommand2.ResetVoiceDetect();
            // }
            
            //Main spells add to vocabulary
            addSpellsToVocabulary(recognizer);
            recognizer.ResultReady.AddListener(res => instance.tryresult(res.text));
            yield return new WaitForSeconds(1f);
            instance.GetComponent<SpeechRecognizer>().StartProcessing();
            while (instance.isActiveAndEnabled)
            {
                yield return new WaitForSeconds(30f);
                var vbt = vbtRef(instance);
                if (!vbt.IsTransmitting)
                {
                    if (recognizer != null && recognizer.State != SpeechProcessorState.Inactive)
                    {
                        recognizer.StopProcessing();
                        instance.StartCoroutine((IEnumerator)restartsrMethod.Invoke(instance, null));
                    }
                }
            }
            yield break;
        }

        private static void addSpellsToVocabulary(SpeechRecognizer recognizer)
        {
            foreach (var pair in russianCommandMap)
            {
                foreach (var word in pair.Key)
                {
                    recognizer.Vocabulary.Add(word);
                }
            }
            foreach (var pair in russianAdditionalCommandMap)
            {
                foreach (string word in pair.Value)
                {
                    recognizer.Vocabulary.Add(word);
                }
            }
        }

        [HarmonyPatch("tryresult")]
        [HarmonyPrefix]
        private static bool TryResultPrefix(VoiceControlListener __instance, string res)
        {
            if (res != null)
            {
                foreach (var command in russianCommandMap)
                {
                    if (command.Key.Any(keyword => res.Contains(keyword)))
                    {
                        command.Value(__instance);
                    }
                }
                foreach (var pair in russianAdditionalCommandMap)
                {
                    if (pair.Value.Any(keyword => res.Contains(keyword)))
                    {
                        var spell = __instance.SpellPages.FirstOrDefault(s =>
                            s != null && s.GetSpellName() == pair.Key);

                        spell?.TryCastSpell();
                        //return;
                    }
                }
                srRef(__instance).StopProcessing();
                __instance.StartCoroutine((IEnumerator)restartsrMethod.Invoke(__instance, null));
            }
            return false;
        }

        [HarmonyPatch("resetmiclong")]
        [HarmonyPrefix]
        private static bool ResetMicLongPrefix(VoiceControlListener __instance, ref IEnumerator __result)
        {
            __result = ModifiedResetMicLong(__instance);
            return false;
        }
        private static IEnumerator ModifiedResetMicLong(VoiceControlListener instance)
        {
            var recognizer = srRef(instance);
            recognizer.StopProcessing();
            yield return new WaitForSeconds(0.5f);
            UnityEngine.Object.Destroy(recognizer);
            srRef(instance) = instance.gameObject.AddComponent<SpeechRecognizer>();
            var recognizerNew = srRef(instance);
            recognizerNew.LanguageModelProvider = instance.GetComponent<StreamingAssetsLanguageModelProvider>();
            recognizerNew.SpeechSource = instance.GetComponent<DissonanceSpeechSource>();
            recognizerNew.Vocabulary = new List<string>();
            instance.SpellPages = new List<ISpellCommand>();
            MonoBehaviour[] components = instance.gameObject.GetComponents<MonoBehaviour>();
            for (int i = 0; i < components.Length; i++)
            {
                ISpellCommand spellCommand = components[i] as ISpellCommand;
                if (spellCommand != null)
                {
                    instance.SpellPages.Add(spellCommand);
                }
            }
            addSpellsToVocabulary(recognizerNew);
            recognizerNew.ResultReady.AddListener(res => instance.tryresult(res.text));
            yield return new WaitForSeconds(0.1f);
            recognizerNew.StartProcessing();
            yield break;
        }
        
        //Some magic error appear after i do resetmic this is because i add this method
        [HarmonyPatch("restartsr")]
        [HarmonyPrefix]
        private static bool RestartSrPrefix(VoiceControlListener __instance, ref IEnumerator __result)
        {
            __result = SafeRestartSr(__instance);
            return false; // Skip original
        }

        private static IEnumerator SafeRestartSr(VoiceControlListener instance)
        {
            var recognizer = srRef(instance);
            if (recognizer == null)
            {
                yield break;
            }
            while (recognizer.State != SpeechProcessorState.Inactive)
            {
                yield return null;
            }
            if (recognizer != null)
            {
                recognizer.StartProcessing();
            }
            yield break;
        }
    }
}