using System;
using System.Collections;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MonoMod.RuntimeDetour;
using UnityEngine;

namespace ChangeSkin
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string ModGUID = "05126619z.changeskin";
        public const string ModName = "ChangeSkin";
        public const string ModVersion = "2.1.0";

        internal static new ManualLogSource Logger;
        private readonly Harmony _harmony = new(ModGUID);
        public static ModConfig ModConfig;
        public static Plugin Instance { get; private set; } = null!;
        public static GameObject SingletonObject;

        public void Awake()
        {
            Logger = base.Logger;
            Instance = this;
            ModConfig = ModConfig.Load(Paths.PluginPath + "/ChangeSkin/settings.json");
            try
            {
                _harmony.PatchAll();
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
            Logger.LogInfo($"Plugin {ModName} is loaded!");
        }

        public void SaveConfig()
        {
            ModConfig.Save(Paths.PluginPath + "/ChangeSkin/settings.json");
        }
    }
}
