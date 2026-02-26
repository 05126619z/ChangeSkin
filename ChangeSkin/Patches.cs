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

    // [HarmonyPatch(typeof(NetBody), nameof(NetBody.OnFoundNetPlayerInitFinish))]
    // internal class NetBody_Patch_OnFoundNetPlayerInitFinish
    // {
    //     public static void Postfix(NetBody __instance)
    //     {
    //         if (
    //             !ChangeSkinMain.replacers.ContainsKey(__instance.clientId)
    //             && !ChangeSkinMain.playerBodies.Contains(__instance)
    //             && ChangeSkinMain.initialized
    //         )
    //         {
    //             ChangeBody changeBody;
    //             if (__instance.body.gameObject.GetComponent<ChangeBody>() == null)
    //             {
    //                 changeBody = __instance.body.gameObject.AddComponent<ChangeBody>();
    //             }
    //             else
    //             {
    //                 changeBody = __instance.body.gameObject.GetComponent<ChangeBody>();
    //             }
    //             ChangeSkinMain.playerBodies.Add(__instance);
    //             ChangeSkinMain.replacers.Add(__instance.clientId, changeBody);
    //         }
    //     }
    // }

    // [HarmonyPatch(typeof(NetBody), nameof(NetBody.OnDestroy))]
    // internal class NetBody_Patch_OnDestroy
    // {
    //     public static void Prefix(NetBody __instance)
    //     {
    //         ChangeSkinMain.playerBodies.Remove(__instance);
    //         ChangeSkinMain.replacers.Remove(__instance.clientId);
    //     }
    // }

    [HarmonyPatch(typeof(ConsoleScript), nameof(ConsoleScript.RegisterAllCommands))]
    internal class ConsoleScript_Patch_RegisterAllCommands
    {
        public static void Postfix()
        {
            ConsoleScript.Commands.Add(
                new Command(
                    "skin",
                    "Control command for ChangeSkin",
                    delegate(string[] args)
                    {
                        string output = ChangeSkinMain.ToggleReplacement(args);
                        ConsoleScript.instance.LogToConsole(output);
                        // Plugin.Logger.LogInfo(output);
                    },
                    null,
                    []
                )
            );
        }
    }

    [HarmonyPatch(typeof(ConsoleScript), nameof(ConsoleScript.TryExecuteCommand))]
    [HarmonyPriority(300)] // Hijack this shit from krok's thing
    internal class ConsoleScript_Patch_TryExecuteCommand
    {
        public static bool Prefix(ConsoleScript __instance, string[] args, bool addToLog)
        {
            if (args.Length > 0 && args[0] == "skin")
            {
                string output = ChangeSkinMain.ToggleReplacement(args);
                __instance.LogToConsole(output);
                __instance.AddCommandToLogAndClearInput();
                return false;
            }
            return true;
        }
    }
}
