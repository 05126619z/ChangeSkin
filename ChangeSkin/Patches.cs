using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Reflection;
using System.Collections;
using UnityEngine.UIElements;
using System.Xml.Linq;
using static UnityEngine.UIElements.UIR.GradientSettingsAtlas;
using System.Threading.Tasks;

namespace ChangeSkin
{
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
    }
}
