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
    // [HarmonyPatch(typeof(Body))]
    // internal class Body_Patches
    // {
    //     [HarmonyPatch(nameof(Body.Update))]
    //     public static void Postfix(Body __instance)
    //     {
    //         ChangeBody changeBody = __instance.GetComponent<ChangeBody>();
    //         if (changeBody != null && changeBody.working)
    //         {
    //             __instance.StartCoroutine(changeBody.ReplaceSprites());
    //         }
    //     }
    // }
    // [HarmonyPatch(typeof(WoundView))]
    // internal class WoundView_Patches
    // {
    //     [HarmonyPatch(nameof(WoundView.Update))]
    //     public static void Postfix(WoundView __instance)
    //     {
    //         if (Config.replaceWoundView)
    //         {
    //             __instance.StartCoroutine(ChangeWoundView.ReplaceSprites());
    //         }
    //     }
    // }
}
