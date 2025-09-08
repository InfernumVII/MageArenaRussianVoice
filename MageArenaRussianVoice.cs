using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MageArenaRussianVoice.Config;

namespace MageArenaRussianVoice;

[BepInPlugin("com.infernumvii.magearenarussianvoice", "MageArenaRussianVoice", "2.1.0")]
public class MageArenaRussianVoice : BaseUnityPlugin
{
    private readonly Harmony harmony = new Harmony("com.infernumvii.magearenarussianvoice");
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;
        VoiceCommandConfig.Init(Config);
        harmony.PatchAll();
        Logger.LogInfo("MageArenaRussianVoice loaded!");
    }
    
    

}
