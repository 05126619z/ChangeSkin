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

    public static Dictionary<ulong, ChangeBody> replacers;
    public static bool initialized = false;
    public static new bool enabled = false;

    public static void FirstInit()
    {
        replacers = [];
        localBody = PlayerCamera.main.body;
        localNetworkComponent = Plugin.SingletonObject.GetComponent<ChangeSkinNetworkComponent>();
        if (!KrokoshaScavMultiplayer.network_system_is_running)
        {
            localChangeBody = localBody.gameObject.AddComponent<ChangeBody>();
            replacers[0] = localChangeBody;
        }
        else
        {
            localScavInstance = ScavClientInstance.local_scavclientinstance;
            scavClientInstances = ScavMultiGlobalSynchronizer.GetAllLivingPlayers();
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

        if (Plugin.ModConfig.LastSelectedSkin != null)
            SkinSelectLocal(localChangeBody, Plugin.ModConfig.LastSelectedSkin);
        if (Plugin.ModConfig.LastURL != null)
            SkinSelectRemote(localChangeBody, Plugin.ModConfig.LastURL);

        ip = KrokoshaScavMultiplayer.input_ipport_text.Split(':')[0];
        port = KrokoshaScavMultiplayer.input_ipport_text.Split(':')[1];
        initialized = true;
    }

    private static void OnSceneUnloaded(Scene scene)
    {
        Destructor();
    }

    private static void Destructor()
    {
        initialized = false;
        localScavInstance = null;
        localChangeBody = null;
        scavClientInstances = null;
        localScavInstance = null;
        localBody = null;
        replacers = [];
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

    public static void SkinSelectLocal(ChangeBody changeBody, string skinName)
    {
        changeBody.LoadSkinLocal(skinName);
    }

    public static void SkinSelectRemote(ChangeBody changeBody, string url)
    {
        changeBody.LoadSkinURL(url);
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
            " skin load local {skinName}\n skin load remote {skinURL}\n skin rule set/get skinuploading true/false\n skin rule set/get skindownloading true/false\n skin ban/unban {playername}\n skin enable/disable\n skin reload\n skin clearcache";
        string returnmessage = helpMessage;

        if (args.Length == 1)
            return returnmessage;

        string command = args[1];

        if (command == "load" && args.Length == 4)
        {
            if (args[2] == "local")
            {
                SkinSelectLocal(localChangeBody, args[3]);
                Plugin.ModConfig.LastSelectedSkin = args[3];
                returnmessage = $"Local skin {args[3]} loaded";
            }
            if (args[2] == "remote")
            {
                SkinSelectRemote(localChangeBody, args[3]);
                Plugin.ModConfig.LastURL = args[3];
                returnmessage = $"Remote skin {args[3]} loaded";
            }
        }

        if (command == "rule" && args.Length == 5)
        {
            if (args[2] == "set")
            {
                if (args[3] == "skinuploading")
                {
                    try
                    {
                        Plugin.ModConfig.SkinUploading = bool.Parse(args[4]);
                        returnmessage = $"Skin uploading is now {Plugin.ModConfig.SkinUploading}";
                    }
                    catch (Exception e)
                    {
                        returnmessage = $"Error: {e}";
                    }
                }
                if (args[3] == "skindownloading")
                {
                    try
                    {
                        Plugin.ModConfig.SkinDownloading = bool.Parse(args[4]);
                        returnmessage =
                            $"Skin downloading is now {Plugin.ModConfig.SkinDownloading}";
                    }
                    catch (Exception e)
                    {
                        returnmessage = $"Error: {e}";
                    }
                }
            }
            if (args[2] == "get")
            {
                if (args[3] == "skinuploading")
                {
                    returnmessage = $"Skin uploading is set to {Plugin.ModConfig.SkinUploading}";
                }
                if (args[3] == "skindownloading")
                {
                    returnmessage =
                        $"Skin downloading is set to {Plugin.ModConfig.SkinDownloading}";
                }
            }
        }

        if (command == "ban" && args.Length == 3)
        {
            foreach (ScavClientInstance scavClientInstance in scavClientInstances)
            {
                if (scavClientInstance.name == args[2])
                {
                    ChangeBody changeBody =
                        scavClientInstance.body.gameObject.GetComponent<ChangeBody>();
                    changeBody.isBanned = true;
                    returnmessage = $"{scavClientInstance.name} is now skinbanned";
                    break;
                }
                else
                    returnmessage = $"{args[2]} not found";
            }
        }

        if (command == "unban" && args.Length == 3)
        {
            foreach (ScavClientInstance scavClientInstance in scavClientInstances)
            {
                if (scavClientInstance.name == args[2])
                {
                    ChangeBody changeBody =
                        scavClientInstance.body.gameObject.GetComponent<ChangeBody>();
                    changeBody.isBanned = false;
                    returnmessage = $"{scavClientInstance.name} is now skinpardoned";
                    break;
                }
                else
                    returnmessage = $"{args[2]} not found";
            }
        }

        if (command == "enable")
        {
            enabled = true;
            foreach (ChangeBody changeBody in replacers.Values)
            {
                changeBody.BeginReplacement();
            }
            returnmessage = "ChangeSkin enabled";
            if (Plugin.ModConfig.LastSelectedSkin == null)
                returnmessage = "Skin for self not selected \notherwise everything is ok";
        }

        if (command == "disable")
        {
            enabled = false;
            foreach (ChangeBody changeBody in replacers.Values)
            {
                changeBody.StopReplacement();
            }
            returnmessage = "ChangeSkin disabled";
        }

        if (command == "reload")
        {
            foreach (ChangeBody changeBody in replacers.Values)
            {
                changeBody.Reload();
            }
            returnmessage = "ChangeSkin reloaded";
        }

        if (command == "clearcache")
        {
            SkinLoader.ClearCache();
            returnmessage = "Cache cleared";
        }

        Plugin.Logger.LogInfo(returnmessage);
        return returnmessage;

        // if (command == "select")
        // {
        //     if (args.Length < 3)
        //         return "Usage: skin select <Folder with skin>";

        //     string skinPath = Paths.PluginPath + "/ChangeSkin/resources" + $"/{args[2]}";
        //     Plugin.ModConfig.LastSelectedSkin = args[2];
        //     SkinSelect(localChangeBody, args[2]);
        //     Plugin.Instance.SaveConfig();
        //     return Directory.Exists(skinPath) ? $"{args[2]} selected" : $"{args[2]} not found";
        // }

        // if (Plugin.ModConfig.LastSelectedSkin == null)
        //     return "Select skin first";

        // if (command == "enable")
        // {
        //     ChangeSkinEnable();
        // }

        // return command switch
        // {
        //     "enable" => ExecuteWithConfig(ChangeSkinEnable, "Texture replacement ON"),
        //     "disable" => ExecuteWithConfig(ChangeSkinDisable, "Texture replacement OFF"),
        //     "reload" => ExecuteWithConfig(ChangeSkinReload, "Textures reloaded"),
        //     _ => helpMessage,
        // };
    }

    private static string ExecuteWithConfig(Action action, string successMessage)
    {
        action();
        return successMessage;
    }
}
