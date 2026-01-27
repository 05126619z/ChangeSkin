using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BepInEx;
using UnityEngine;

namespace ChangeSkin;

internal static class Config
{
    public static bool replaceBody = false;
    public static bool replaceWoundView = false;
    public static string skinName;

    public static string ToggleReplacement(string[] args)
    {
        string helpMessage =
            "enable to toggle ON, disable to toggle OFF, reload to reload, select <skin name> to select skin";
        
        if (args.Length == 1)
            return helpMessage;

        string command = args[1];

        if (command == "select")
        {
            if (args.Length < 3)
                return "Usage: skin select <Folder with skin>";

            string skinPath = Paths.PluginPath + "/ChangeSkin/resources" + $"/{args[2]}";
            Plugin.ModConfig.CachedSkinName = args[2];
            skinName = args[2];
            return Directory.Exists(skinPath) ? $"{args[2]} selected" : $"{args[2]} not found";
        }

        if (skinName == null && Plugin.ModConfig.CachedSkinName != null)
        {
            skinName = Plugin.ModConfig.CachedSkinName;
        }

        if (skinName == null)
            return "Select skin first";

        return command switch
        {
            "enable" => ExecuteWithConfig(Config.ToggleOn, "Texture replacement ON"),
            "disable" => ExecuteWithConfig(Config.ToggleOff, "Texture replacement OFF"),
            "reload" => ExecuteWithConfig(Config.Reload, "Textures reloaded"),
            _ => helpMessage,
        };
    }

    private static string ExecuteWithConfig(Action action, string successMessage)
    {
        action();
        return successMessage;
    }

    private static readonly Action[] OnActions = [ChangeBody.ToggleOn, ChangeWoundView.ToggleOn];

    private static readonly Action[] OffActions = [ChangeBody.ToggleOff, ChangeWoundView.ToggleOff];

    public static void ToggleOn()
    {
        foreach (Action action in OnActions)
        {
            action.Invoke();
        }
    }

    public static void ToggleOff()
    {
        foreach (Action action in OffActions)
        {
            action.Invoke();
        }
    }

    public static void Reload()
    {
        ToggleOff();
        ToggleOn();
    }
}
