using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MonoMod.RuntimeDetour;
using UnityEngine;

namespace ChangeSkin
{
    [BepInPlugin("05126619z.changeskin", "ChangeSkin", "1.3.0")]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        private readonly Harmony _harmony = new("05126619z.changeskin");
        public static ModConfig ModConfig;

        public void Awake()
        {
            Logger = base.Logger;
            ModConfig = ModConfig.Load(Paths.PluginPath + "/ChangeSkin/settings.json");
            try
            {
                Hook a1 = new(
                    typeof(Body).GetMethod(
                        "Update",
                        BindingFlags.NonPublic | BindingFlags.Instance
                    ),
                    typeof(Patches).GetMethod(nameof(Patches.Body_Update))
                );
                Hook a2 = new(
                    typeof(WoundView).GetMethod(
                        "Update",
                        BindingFlags.NonPublic | BindingFlags.Instance
                    ),
                    typeof(Patches).GetMethod(nameof(Patches.WoundView_Update))
                );
                // Hook a3 = new(
                //     typeof(ConsoleScript).GetMethod(
                //         "Start",
                //         BindingFlags.Public | BindingFlags.Instance
                //     ),
                //     typeof(Patches).GetMethod(nameof(Patches.ConsoleScript_Start)) // I didnt want to add this but no commands register due to bug in ConsoleScript.Start()  !!  Remove when patched.
                // );
                Logger.LogInfo(a1);
                Logger.LogInfo(a2);
                // Logger.LogInfo(a3);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
            _harmony.PatchAll();
            ScavHook.ConsoleManager.AddCommand("skin", args => ChangeSkin.Config.ToggleReplacement(args));

            Logger.LogInfo($"Plugin ChangeSkin is loaded!");
        }
    }
}
