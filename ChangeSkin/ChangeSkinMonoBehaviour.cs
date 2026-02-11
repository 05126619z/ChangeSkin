using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ChangeSkin;

public class ChangeSkinMonoBehaviour : MonoBehaviour
{
    public static Body localBody;
    public static ChangeBody localChangeBody;
    public static bool initialized = false;

    public static void Init()
    {
        TextureStorage.SaveOGSprites();
        localBody = PlayerCamera.main.body;
        localChangeBody = localBody.gameObject.AddComponent<ChangeBody>();
        localChangeBody.isLocalChangeBody = true;

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
        localBody = null;
        localChangeBody = null;
        TextureStorage.ogSprites = [];
    }

    public static void SkinSelectLocal(ChangeBody changeBody, string skinName)
    {
        changeBody.LoadSkinLocal(skinName);
    }

    public static void SkinSelectRemote(ChangeBody changeBody, string url)
    {
        changeBody.LoadSkinURL(url);
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
                Plugin.ModConfig.lastSelected = ModConfig.LastSelected.Local;
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
                    Plugin.ModConfig.lastSelected = ModConfig.LastSelected.Remote;
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

        if (command == "enable")
        {
            localChangeBody.BeginReplacement();
            returnmessage = "ChangeSkin enabled";
            if (Plugin.ModConfig.LastSelectedSkin == null)
                returnmessage = "Skin for self not selected \notherwise everything is ok";
        }

        if (command == "disable")
        {
            localChangeBody.StopReplacement();
            returnmessage = "ChangeSkin disabled";
        }

        if (command == "reload")
        {
            localChangeBody.Reload();
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

        if (command == "reinit")
        {
            Destructor();
            Init();
        }

        Plugin.Logger.LogInfo(returnmessage);
        return returnmessage;
    }
}
