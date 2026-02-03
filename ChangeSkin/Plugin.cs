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
    [BepInPlugin("05126619z.changeskin", "ChangeSkin", "1.3.1")]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        // public static AssetBundle myAssetBundle;
        public static ModConfig ModConfig;

        private readonly Harmony _harmony = new("05126619z.changeskin");

        //private static string AssetsFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public void Awake()
        {
            //myAssetBundle = AssetBundle.LoadFromFile(Path.Combine(AssetsFolderPath, "myassetbundle"));
            Logger = base.Logger;
            ModConfig = ModConfig.Load(Paths.PluginPath + "/ChangeSkin/settings.json");
            try
            {
                _harmony.PatchAll();
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
                Logger.LogInfo(a1);
                Logger.LogInfo(a2);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
            Logger.LogInfo($"Plugin ChangeSkin is loaded!");
        }
    }
}
