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
using KrokoshaCasualtiesMP;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using UnityEngine.Video;
using static UnityEngine.UIElements.UIR.GradientSettingsAtlas;

namespace ChangeSkin
{
    [HarmonyPatch(typeof(PlayerBody))]
    internal class PlayerBody_Patch1
    {
        [HarmonyPatch(nameof(PlayerBody.OnFoundScavClientInstanceInitFinish))]
        public static void Postfix(PlayerBody __instance)
        {
            ChangeSkinMonoBehaviour.PlayerBodies.Add(__instance);
            ChangeBody changeBody = __instance.sci.body.gameObject.AddComponent<ChangeBody>();
            ChangeSkinMonoBehaviour.replacers.Add(__instance.clientId, changeBody);
            if (__instance.sci == ScavClientInstance.local_scavclientinstance)
            {
                changeBody.isLocalChangeBody = true;
                ChangeSkinMonoBehaviour.localChangeBody = changeBody;
                ChangeSkinMonoBehaviour.localPlayerBody = __instance;
                ChangeSkinMonoBehaviour.localBody = __instance.body;
            }
        }
    }

    [HarmonyPatch(typeof(PlayerBody))]
    internal class PlayerBody_Patch2
    {
        [HarmonyPatch(nameof(PlayerBody.OnDestroy))]
        public static void Prefix(PlayerBody __instance)
        {
            ChangeSkinMonoBehaviour.PlayerBodies.Remove(__instance);
            ChangeSkinMonoBehaviour.replacers.Remove(__instance.clientId);
        }
    }
}
