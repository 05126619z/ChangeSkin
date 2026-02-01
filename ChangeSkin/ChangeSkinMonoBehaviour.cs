using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using KrokoshaCasualtiesMP;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ChangeSkin;

public class ChangeSkinMonoBehaviour : MonoBehaviour
{
    static string localSkin;
    static ScavClientInstance localScavInstance;
    static Body localBody;
    static ChangeBody localChangeBody;
    static Dictionary<ScavClientInstance, ChangeBody> replacers = new();
    static List<ScavClientInstance> scavClientInstances;
    static bool initialized = false;

    public static void FirstInit()
    {
        localSkin = Plugin.ModConfig.SelectedSkin ?? localSkin;
        localScavInstance = ScavClientInstance.local_scavclientinstance;
        localBody = PlayerCamera.main.body;
        if (!KrokoshaScavMultiplayer.network_system_is_running)
        {
            localBody.gameObject.AddComponent<ChangeBody>();
        }
        scavClientInstances =
            ScavMultiClientSynchronizer.ClientIdToScavClientInstanceDict.Values.ToList<ScavClientInstance>();
        foreach (ScavClientInstance scavClientInstance in scavClientInstances)
        {
            scavClientInstance.chara.AddComponent<ChangeBody>();
        }
        SceneManager.sceneUnloaded += new UnityAction<Scene>(OnSceneUnloaded);
        localChangeBody = localBody.gameObject.GetComponent<ChangeBody>();
        SkinSelect(localChangeBody, localSkin);
        initialized = true;
    }

    public static void OnSceneUnloaded(Scene scene)
    {
        localScavInstance = null;
        scavClientInstances = null;
        localBody = null;
    }

    public static void ChangeSkinEnable()
    {
        localChangeBody.BeginReplacement();
    }

    public static void ChangeSkinDisable()
    {
        localChangeBody.StopReplacement();
    }

    public static void SkinSelect(ChangeBody changeBody, string skinName)
    {
        changeBody.LoadSkin(skinName);
    }

    public static void ToggleOn()
    {
        // foreach(ScavClientInstance scavClientInstance in ClientIdToScavClientInstanceDict.Values)
        // {
        //     ChangeBody changeBody = scavClientInstance.body.gameObject.AddComponent<ChangeBody>();

        //     changeBody.SetFromFolder();
        // }
    }

    public static void ToggleOff() { }

    public static void ChangeSkinReload()
    {
        ToggleOff();
        ToggleOn();
    }

    public static void Startcorout(IEnumerator f)
    {
        Plugin.SingletonObject.GetComponent<ChangeSkinMonoBehaviour>().StartCoroutine(f);
    }

    public static void Stopcorout(IEnumerator f)
    {
        Plugin.SingletonObject.GetComponent<ChangeSkinMonoBehaviour>().StopCoroutine(f);
    }

    public static string ToggleReplacement(string[] args)
    {
        if (!initialized)
            FirstInit();

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
            Plugin.ModConfig.SelectedSkin = args[2];
            localSkin = args[2];
            SkinSelect(localChangeBody, args[2]);
            return Directory.Exists(skinPath) ? $"{args[2]} selected" : $"{args[2]} not found";
        }

        if (localSkin == null)
            return "Select skin first";

        return command switch
        {
            "enable" => ExecuteWithConfig(ChangeSkinEnable, "Texture replacement ON"),
            "disable" => ExecuteWithConfig(ChangeSkinDisable, "Texture replacement OFF"),
            "reload" => ExecuteWithConfig(ChangeSkinReload, "Textures reloaded"),
            _ => helpMessage,
        };
    }

    private static string ExecuteWithConfig(Action action, string successMessage)
    {
        action();
        return successMessage;
    }
}
