using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ChangeSkin
{
    internal static class Config
    {
        public static bool replaceBody = false;
        public static bool replaceWoundView = false;
        public static string skinName;
        public static string ToggleReplacement(string[] args) // For console
        {
            if (args.Length == 1)
            {
                return "enable to toggle ON, disable to toggle OFF, reload to reload, select <skin name> to select skin";
            }
            switch (args[1])
            {
                case "enable":
                    if (skinName == null)
                    {
                        return "Select skin first";
                    }
                    Config.ToggleOn();
                    return "Texture replacement ON";
                case "disable":
                    if (skinName == null)
                    {
                        return "Select skin first";
                    }
                    Config.ToggleOff();
                    return "Texture replacement OFF";
                case "reload":
                    if (skinName == null)
                    {
                        return "Select skin first";
                    }
                    Config.Reload();
                    return "Textures reloaded";
                case "select":

                    if (args[2] == null)
                    {
                        return "Usage: skin select <Folder with skin>";
                    }
                    if (Directory.Exists(string.Concat(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"/{args[2]}")))
                    {
                        skinName = args[2];
                        return $"{skinName} selected";
                    }
                    else
                    {
                        return $"{skinName} not found";
                    }
                default:
                    return "enable to toggle ON, disable to toggle OFF, reload to reload, select <skin name> to select skin";
            }
        }
        internal static void ToggleOn()
        {
            ChangeBody.ToggleOn();
            // ChangeWoundView.ToggleOn();
        }
        internal static void ToggleOff()
        {
            ChangeBody.ToggleOff();
            // ChangeWoundView.ToggleOff();
        }
        internal static void Reload()
        {

            ChangeBody.ToggleOff();
            // ChangeWoundView.ToggleOff();
            ChangeBody.ToggleOn();
            // ChangeWoundView.ToggleOn();
        }
    }
}
