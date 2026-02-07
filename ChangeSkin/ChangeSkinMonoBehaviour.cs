using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using KrokoshaCasualtiesMP;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ChangeSkin;

public class ChangeSkinMonoBehaviour : MonoBehaviour
{
    static ChangeSkinNetworkComponent localNetworkComponent;
    static ScavClientInstance localScavInstance;
    static Body localBody;
    static ChangeBody localChangeBody;
    static List<ScavClientInstance> scavClientInstances;
    static string ip;
    static string port;

    public static string localSkin;
    public static Dictionary<ulong, ChangeBody> replacers = new();
    public static bool initialized = false;

    public static void FirstInit()
    {
        if (Plugin.ModConfig.LastSelectedSkin != null)
            localSkin = Plugin.ModConfig.LastSelectedSkin;
        localBody = PlayerCamera.main.body;

        if (!KrokoshaScavMultiplayer.network_system_is_running)
        {
            localChangeBody = localBody.gameObject.AddComponent<ChangeBody>();
        }
        else
        {
            localScavInstance = ScavClientInstance.local_scavclientinstance;
            scavClientInstances =
                ScavMultiClientSynchronizer.ClientIdToScavClientInstanceDict.Values.ToList<ScavClientInstance>();
            foreach (ScavClientInstance scavClientInstance in scavClientInstances)
            {
                ChangeBody changeBody =
                    scavClientInstance.body.gameObject.AddComponent<ChangeBody>();
                replacers.Add(
                    ScavClientInstance.GetClientIdFromBody(scavClientInstance.body),
                    changeBody
                );
                if (localScavInstance == scavClientInstance)
                {
                    localChangeBody = changeBody;
                }
            }
            SceneManager.sceneUnloaded += new UnityAction<Scene>(OnSceneUnloaded);
            // NetworkManager.Singleton.OnClientConnectedCallback += new Action<ulong>(
            //     ChangeSkinClientConnect
            // );
            // NetworkManager.Singleton.OnClientDisconnectCallback += new Action<ulong>(
            //     ChangeSkinClientDisconnect
            // );
        }
        if (localSkin != null)
            SkinSelect(localChangeBody, localSkin);

        ip = KrokoshaScavMultiplayer.input_ipport_text.Split(':')[0];
        port = KrokoshaScavMultiplayer.input_ipport_text.Split(':')[1];
        initialized = true;
    }

    private static void OnSceneUnloaded(Scene scene)
    {
        initialized = false;
        localScavInstance = null;
        localChangeBody = null;
        scavClientInstances = null;
        localScavInstance = null;
        localBody = null;
        replacers = null;
        // Really retarded solution by keeping everything static, but idgaf so thats my destructor
    }

    private static void ChangeSkinClientConnect(ulong clientId)
    {
        ChangeBody changeBody = ScavClientInstance
            .GetBodyFromClientId(clientId)
            .gameObject.AddComponent<ChangeBody>();
        replacers[clientId] = changeBody;
    }

    private static void ChangeSkinClientDisconnect(ulong clientId)
    {
        replacers.Remove(clientId);
    }

    public static void ChangeSkinEnable()
    {
        localChangeBody.BeginReplacement();
        if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsClient())
        {
            KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer("ChangeSkinInitMessage");
        }
        else { }
    }

    public static void ChangeSkinDisable()
    {
        localChangeBody.StopReplacement();
    }

    public static void SkinSelect(ChangeBody changeBody, string skinName)
    {
        changeBody.LoadSkinLocal(skinName);
    }

    public static void ChangeSkinReload()
    {
        ChangeSkinEnable();
        ChangeSkinDisable();
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
            Plugin.ModConfig.LastSelectedSkin = args[2];
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
