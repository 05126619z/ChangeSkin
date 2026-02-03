using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using UnityEngine.Video;
using static UnityEngine.UIElements.UIR.GradientSettingsAtlas;

namespace ChangeSkin
{
    [HarmonyPatch]
    internal class Patches
    {
        public delegate void orig_Body_Update(Body self);

        public static void Body_Update(orig_Body_Update orig, Body self)
        {
            orig(self);
            if (Config.replaceBody)
            {
                self.StartCoroutine(ChangeBody.ReplaceSprites());
            }
        }

        public delegate void orig_WoundView_Update(WoundView self);

        public static void WoundView_Update(orig_WoundView_Update orig, WoundView self)
        {
            orig(self);
            if (Config.replaceWoundView)
            {
                self.StartCoroutine(ChangeWoundView.ReplaceSprites());
            }
        }

        [HarmonyPatch(typeof(ConsoleScript), nameof(ConsoleScript.RegisterAllCommands))]
        public static void Postfix()
        {
            ConsoleScript.Commands.Add(
                new Command(
                    "skin",
                    "Control command for ChangeSkin",
                    delegate(string[] args)
                    {
                        string output = ChangeSkin.Config.ToggleReplacement(args);
                        ConsoleScript.instance.LogToConsole(output);
                        Plugin.Logger.LogInfo(output);
                    },
                    null,
                    []
                )
            );
        }
    }
}
