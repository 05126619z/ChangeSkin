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
        public static void AddCommand(string commandName, Func<string[], string> commandFunction)
        {
            ConsoleStuff.addedCommands[commandName] = commandFunction;
        }
    }

    [HarmonyPatch(typeof(ConsoleScript))]
    [HarmonyPatch(nameof(ConsoleScript.CheckInput))]
    internal class ConsoleScriptHook
    {
        public static bool Prefix(
            ConsoleScript __instance,
            ref string __result
        )
        {
            string text = __instance.input.text;
            string command = text.Split(' ')[0];
            if (command.IsNullOrWhiteSpace())
            {
                __result = "Command does not exist";
                return false;
            }
            for (int i = 0; i < ConsoleStuff.builtInCommands.Length; i++)
            {
                if (command == ConsoleStuff.builtInCommands[i])
                {
                    return true;
                }
            }
            for (int i = 0; ConsoleStuff.addedCommands.Count > i; i++)
            {
                if (ConsoleStuff.addedCommands.ContainsKey(command))
                {
                    ConsoleStuff.addedCommands.TryGetValue(
                        command,
                        out Func<string[], string> Function
                    );
                    string[] args = text.Split(' ').ToArray();
                    __result = Function(args);
                    return false;
                }
            }
            return true;
        }
    }

    internal class ConsoleStuff
    {
        internal static string[] builtInCommands { get; private set; } =
            {
                "setvolume",
                "sound",
                "help",
                "talk",
                "heal",
                "spawn",
                "setbodyfield",
                "tp",
                "getlimbs",
                "setlimbfield",
                "getsounds",
                "framerate",
                "pixelate",
                "starterkit",
                "getlimbfields",
                "timescale",
                "skiplayer",
                "getbodyfields",
                "explode",
            };

        internal static Dictionary<string, Func<string[], string>> addedCommands =
            new Dictionary<string, Func<string[], string>>();
    }
}
