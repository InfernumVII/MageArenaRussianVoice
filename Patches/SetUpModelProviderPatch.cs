using System;
using System.Collections.Generic;
using System.ComponentModel;
using HarmonyLib;
using Recognissimo.Components;
using UnityEngine;


namespace MageArenaRussianVoice.Patches
{
    [HarmonyPatch(typeof(SetUpModelProvider), "Setup")]
    public static class SetUpModelProviderPatch
    {
        private static readonly string nameOfModel = "vosk-model-small-ru-0.22";
        private static readonly string modPath = System.IO.Path.GetDirectoryName(typeof(SetUpModelProviderPatch).Assembly.Location);
        private static readonly string modelPath = System.IO.Path.Combine(modPath, $"LanguageModels/{nameOfModel}");

        [HarmonyPrefix]
        public static bool Prefix(SetUpModelProvider __instance)
        {
            StreamingAssetsLanguageModelProvider streamingAssetsLanguageModelProvider = __instance.gameObject.AddComponent<StreamingAssetsLanguageModelProvider>();
            streamingAssetsLanguageModelProvider.language = SystemLanguage.Russian;
            streamingAssetsLanguageModelProvider.languageModels = new List<StreamingAssetsLanguageModel>
            {
                new StreamingAssetsLanguageModel
                {
                    language = SystemLanguage.Russian,
                    path = modelPath
                }
            };
            SpeechRecognizer speechRecognizer = __instance.GetComponent<SpeechRecognizer>();
            speechRecognizer.LanguageModelProvider = streamingAssetsLanguageModelProvider;
            return false;
        }
    }
}
