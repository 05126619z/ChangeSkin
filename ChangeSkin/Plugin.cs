using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using MonoMod.RuntimeDetour;
using UnityEngine;

namespace ChangeSkin
{
    [BepInPlugin("05126619z.changeskin", "ChangeSkin", "1.0.0")]
    public partial class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource logger;
        public static AssetBundle myAssetBundle;
        //private static string AssetsFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public void Awake()
        {
            //myAssetBundle = AssetBundle.LoadFromFile(Path.Combine(AssetsFolderPath, "myassetbundle"));
            logger = base.Logger;
            try
            {
                Hook a1 = new Hook(
                    typeof(Body).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance),
                    typeof(Patches).GetMethod(nameof(Patches.Body_Update))
                    );
                logger.LogInfo(a1);
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }
            ScavLib.Commands.ConsoleManager.AddCommand("skin", args => Patches.ToggleReplacement(args));

            logger.LogInfo($"Plugin ChangeSkin is loaded!");
        }
    }
}
