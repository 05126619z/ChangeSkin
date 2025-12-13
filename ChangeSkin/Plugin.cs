using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using MonoMod.RuntimeDetour;
using UnityEngine;

namespace ChangeSkin
{
    [BepInPlugin("05126619z.changeskin", "ChangeSkin", "1.2.0")]
    public partial class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        public static AssetBundle myAssetBundle;
        //private static string AssetsFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public void Awake()
        {
            //myAssetBundle = AssetBundle.LoadFromFile(Path.Combine(AssetsFolderPath, "myassetbundle"));
            Logger = base.Logger;
            try
            {
                Hook a1 = new Hook(
                    typeof(Body).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance),
                    typeof(Patches).GetMethod(nameof(Patches.Body_Update))
                    );
                    Hook a2 = new Hook(
                    typeof(WoundView).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance),
                    typeof(Patches).GetMethod(nameof(Patches.WoundView_Update))
                    );
                Logger.LogInfo(a1);
                Logger.LogInfo(a2);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
            ConsoleScript.Commands.Add(new Command("skin", "Control command for ChangeSkin", delegate(string[] args)
            {
                Logger.LogInfo(ChangeSkin.Config.ToggleReplacement(args));
            }, null, Array.Empty<ValueTuple<string, string>>()));

            Logger.LogInfo($"Plugin ChangeSkin is loaded!");
        }
    }
}
