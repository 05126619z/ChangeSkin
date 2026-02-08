// Easy to use implementation of ConsoleScript hook for Casualties Unknown
// Works for versions 5.4 and lower
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BepInEx;
using HarmonyLib;
using TMPro;

namespace ScavHook
{
    public static class ConsoleManager
    {
        internal static Dictionary<string, Func<string[], string>> addedCommands =
            new Dictionary<string, Func<string[], string>>();

        public static void AddCommand(string commandName, Func<string[], string> commandFunction)
        {
            addedCommands[commandName] = commandFunction;
        }
    }

    [HarmonyPatch(typeof(ConsoleScript))]
    [HarmonyPatch(nameof(ConsoleScript.CheckInput))]
    [HarmonyPriority(300)] // fuck you krok for breaking console in .12
    internal class ConsoleScriptHook
    {
        public static bool Prefix(ConsoleScript __instance, ref string __result)
        {
            string text = __instance.input.text;
            string[] args = text.Split(' ');
            foreach (
                KeyValuePair<
                    string,
                    Func<string[], string>
                > keyValuePair in ConsoleManager.addedCommands
            )
            {
                if (args[0] == keyValuePair.Key)
                {
                    Func<string[], string> func = keyValuePair.Value;
                    __result = func(args);
                    return false;
                }
            }
            return true;

            // if (commands.Length==0)
            // {
            //     __result = "Command does not exist";
            //     return false;
            // }
            // for (int i = 0; i < ConsoleStuff.builtInCommands.Length; i++)
            // {
            //     if (command == ConsoleStuff.builtInCommands[i])
            //     {
            //         return true;
            //     }
            // }
            // for (int i = 0; ConsoleStuff.addedCommands.Count > i; i++)
            // {
            //     if (ConsoleStuff.addedCommands.ContainsKey(command))
            //     {
            //         ConsoleStuff.addedCommands.TryGetValue(
            //             command,
            //             out Func<string[], string> Function
            //         );
            //         string[] args = text.Split(' ');
            //         __result = Function(args);
            //         return false;
            //     }
            // }
            // return true;
        }
    }
}
