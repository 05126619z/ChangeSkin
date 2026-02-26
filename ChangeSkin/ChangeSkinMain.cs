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

public static class ChangeSkinMain
{
    public static NetBody localPlayerBody;
    public static Body localBody;
    public static ChangeBody localChangeBody;
    public static List<NetBody> playerBodies = [];

    public static Dictionary<ulong, ChangeBody> replacers = replacers = [];
    public static bool initialized = false;

    public static void Init()
    {
        if (!KrokoshaScavMultiplayer.network_system_is_running)
        {
            localBody = PlayerCamera.main.body;
            localChangeBody = localBody.gameObject.AddComponent<ChangeBody>();
            replacers[0] = localChangeBody;
            localChangeBody.isLocalChangeBody = true;
        }
        else
        {
            ChangeSkinNetworkComponent.RegisterServerRecievers();
            ChangeSkinNetworkComponent.RegisterClientRecievers();
            foreach (NetPlayer scavClientInstance in ServerMain.GetAllLivingPlayers())
            {
                playerBodies.Add(scavClientInstance.playerbody);
                ChangeBody changeBody =
                    scavClientInstance.body.gameObject.GetComponent<ChangeBody>();
                if (changeBody == null)
                {
                    changeBody = scavClientInstance.body.gameObject.AddComponent<ChangeBody>();
                }
                replacers.Add(scavClientInstance.playerbody.clientId, changeBody);
                if (scavClientInstance == NetPlayer.LOCAL_PLAYER)
                {
                    changeBody.isLocalChangeBody = true;
                    localChangeBody = changeBody;
                    localPlayerBody = scavClientInstance.playerbody;
                    localBody = scavClientInstance.body;
                }
            }
        }

        SceneManager.sceneUnloaded += new UnityAction<Scene>(OnSceneUnloaded);

        switch (Plugin.ModConfig.lastSelected)
        {
            case ModConfig.LastSelected.Local:
            {
                if (Plugin.ModConfig.LastSelectedSkin != null)
                    SkinSelectLocal(localChangeBody, Plugin.ModConfig.LastSelectedSkin);
                break;
            }
            case ModConfig.LastSelected.Remote:
            {
                if (Plugin.ModConfig.LastURL != null)
                    SkinSelectRemote(localChangeBody, Plugin.ModConfig.LastURL);
                break;
            }
            default:
            {
                break;
            }
        }
        initialized = true;
    }

    private static void OnSceneUnloaded(Scene scene)
    {
        Destructor();
    }

    public static void Destructor()
    {
        initialized = false;
        replacers = [];
        playerBodies = [];
        localPlayerBody = null;
        localBody = null;
        localChangeBody = null;
    }

    public static void SkinSelectLocal(ChangeBody changeBody, string skinName)
    {
        changeBody.LoadSkinLocal(skinName);
    }

    public static void SkinSelectRemote(ChangeBody changeBody, string url)
    {
        changeBody.LoadSkinURL(url);
    }

    public static string ToggleReplacement(string[] args)
    {
        if (!initialized)
            Init();

        string helpMessage =
            " skin load local {skinName}\n skin load remote {skinURL}\n skin rule set/get skinuploading true/false\n skin rule set/get skindownloading true/false\n skin ban/unban {playername}\n skin enable/disable\n skin reload\n skin clearcache\n skin verbose true/false\n skin unload";
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
                ChangeSkinNetworkComponent.SendLocalSkinMessage(args[3]);
                returnmessage = $"Local skin {args[3]} loaded";
            }
            if (args[2] == "remote")
            {
                if (!Plugin.ModConfig.SkinDownloading)
                {
                    returnmessage = "Skin downloading is disabled by the rules";
                }
                else
                {
                    SkinSelectRemote(localChangeBody, args[3]);
                    Plugin.ModConfig.LastURL = args[3];
                    ChangeSkinNetworkComponent.SendRemoteSkinMessage(
                        args[3],
                        localChangeBody.skinName
                    );
                    returnmessage = $"Remote skin {args[3]} loaded";
                }
            }
        }

        if (command == "rule")
        {
            if (args[2] == "set" && args.Length == 5)
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
            if (args[2] == "get" && args.Length == 4)
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
            foreach (NetBody playerBody in playerBodies)
            {
                if (playerBody.name == args[2])
                {
                    ChangeBody changeBody = playerBody.body.gameObject.GetComponent<ChangeBody>();
                    changeBody.isBanned = true;
                    changeBody.Unload();
                    returnmessage = $"{playerBody.name} is now skinbanned";
                    break;
                }
                else
                    returnmessage = $"{args[2]} not found";
            }
        }

        if (command == "unban" && args.Length == 3)
        {
            foreach (NetBody playerBody in playerBodies)
            {
                if (playerBody.name == args[2])
                {
                    ChangeBody changeBody = playerBody.body.gameObject.GetComponent<ChangeBody>();
                    changeBody.isBanned = false;
                    returnmessage = $"{playerBody.name} is now skinpardoned";
                    break;
                }
                else
                    returnmessage = $"{args[2]} not found";
            }
        }

        if (command == "enable")
        {
            ChangeSkinNetworkComponent.SendSkinEnabled();
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
            foreach (ChangeBody changeBody in replacers.Values)
            {
                changeBody.StopReplacement();
            }
            ChangeSkinNetworkComponent.SendSkinDisabled();
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

        if (command == "unload")
        {
            localChangeBody.Unload();
            returnmessage = "Self skin unloaded";
        }

        if (command == "clearcache")
        {
            SkinLoader.ClearCache();
            returnmessage = "Cache cleared";
        }

        if (command == "verbose" && args.Length == 3)
        {
            Plugin.ModConfig.Verbose = bool.Parse(args[2]);
        }

        if (command == "init")
        {
            Destructor();
            Init();
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
