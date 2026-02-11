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
//     [HarmonyPatch(typeof(PlayerBody))]
//     internal class PlayerBody_Patch1
//     {
//         [HarmonyPatch(nameof(PlayerBody.OnFoundScavClientInstanceInitFinish))]
//         public static void Postfix(PlayerBody __instance)
//         {
//             if (!ChangeSkinMonoBehaviour.playerBodies.Contains(__instance))
//                 ChangeSkinMonoBehaviour.playerBodies.Add(__instance);
//             ChangeBody changeBody = __instance.body.gameObject.GetComponent<ChangeBody>();
//             if (changeBody == null)
//             {
//                 changeBody = __instance.body.gameObject.AddComponent<ChangeBody>();
//             }
//             ChangeSkinMonoBehaviour.replacers[__instance.clientId] = changeBody;
//             if (__instance.sci == ScavClientInstance.local_scavclientinstance)
//             {
//                 changeBody.isLocalChangeBody = true;
//                 ChangeSkinMonoBehaviour.localChangeBody = changeBody;
//                 ChangeSkinMonoBehaviour.localPlayerBody = __instance;
//                 ChangeSkinMonoBehaviour.localBody = __instance.body;
//             }
//         }
//     }

    [HarmonyPatch(typeof(ConsoleScript))]
    internal class ConsoleScriptAddCommand
    {
        [HarmonyPatch(nameof(ConsoleScript.RegisterAllCommands))]
        public static void Postfix()
        {
            ConsoleScript.Commands.Add(
                new Command(
                    "skin",
                    "Control command for ChangeSkin",
                    delegate(string[] args)
                    {
                        string output = ChangeSkinMonoBehaviour.ToggleReplacement(args);
                        ConsoleScript.instance.LogToConsole(output);
                        // Plugin.Logger.LogInfo(output);
                    },
                    null,
                    []
                )
            );
        }
    }
}
