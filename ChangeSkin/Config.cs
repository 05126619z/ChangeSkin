using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeSkin
{
    internal static class Config
    {
        public static bool replaceBody = false;
        public static bool replaceWoundView = false;
        public static string ToggleReplacement(string[] args) // For console
        {

            if (args.Length == 0)
            {
                return "1 to toggle ON, 0 to toggle OFF, reload to reload";
            }
            if (args[0] == "1")
            {
                Config.ToggleOn();
                return "Texture replacement ON";
            }
            else if (args[0] == "0")
            {
                Config.ToggleOff();
                return "Texture replacement OFF";
            }
            else if (args[0] == "reload")
            {
                Config.Reload();
                return "Textures reloaded";
            }
            return "1 to toggle ON, 0 to toggle OFF, reload to reload";
        }
        internal static void ToggleOn()
        {
            ChangeBody.ToggleOn();
            // ChangeWoundView.ToggleOn();
        }
        internal static void ToggleOff()
        {
            ChangeBody.ToggleOff();
        }
        internal static void Reload()
        {

            ChangeBody.ToggleOff();
            ChangeBody.ToggleOn();
        }
    }
}
