using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using MonoMod.RuntimeDetour;
using UnityEngine;

namespace ChangeSkin
{
    [BepInPlugin("05126619z.changeskin", "ChangeSkin", "1.3.0")]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        // public static AssetBundle myAssetBundle;
        public static ModConfig ModConfig;

        //private static string AssetsFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public void Awake()
        {
            //myAssetBundle = AssetBundle.LoadFromFile(Path.Combine(AssetsFolderPath, "myassetbundle"));
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
                Hook a3 = new(
                    typeof(ConsoleScript).GetMethod(
                        "Start",
                        BindingFlags.Public | BindingFlags.Instance
                    ),
                    typeof(Patches).GetMethod(nameof(Patches.ConsoleScript_Start)) // I didnt want to add this but no commands register due to bug in ConsoleScript.Start()  !!  Remove when patched.
                );
                Logger.LogInfo(a1);
                Logger.LogInfo(a2);
                Logger.LogInfo(a3);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
            ConsoleScript.Commands.Add(
                new Command(
                    "skin",
                    "Control command for ChangeSkin",
                    delegate(string[] args)
                    {
                        string output = ChangeSkin.Config.ToggleReplacement(args);
                        ConsoleScript.instance.LogToConsole(output);
                        Logger.LogInfo(output);
                    },
                    null,
                    []
                )
            );

            Logger.LogInfo($"Plugin ChangeSkin is loaded!");
        }
    }
}
