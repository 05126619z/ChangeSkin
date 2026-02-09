using System;
using System.Collections.Generic;
using KrokoshaCasualtiesMP;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace ChangeSkin;

public static class ChangeSkinNetworkComponent
{
    const string uploadApiUrl = "https://skin.cat-bot.de/ots";

    public static void SendLocalSkinMessage(string skinName)
    {
        string localSkinUrl = ChangeBody.UploadLocalSkin(skinName, uploadApiUrl);
        if (localSkinUrl == null)
            return;
        FastBufferWriter fastBufferWriter = new FastBufferWriter(256, Allocator.Temp, 1200);
        fastBufferWriter.WriteValueSafe(skinName);
        fastBufferWriter.WriteValueSafe(localSkinUrl);
        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(
            "SkinUpdate",
            NetworkManager.ServerClientId,
            fastBufferWriter
        );
    }

    public static void SendRemoteSkinMessage(string url, string skinName)
    {
        FastBufferWriter fastBufferWriter = new FastBufferWriter(256, Allocator.Temp, 1200);
        fastBufferWriter.WriteValueSafe(skinName);
        fastBufferWriter.WriteValueSafe(url);
        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(
            "SkinUpdate",
            NetworkManager.ServerClientId,
            fastBufferWriter
        );
    }

    enum SkinState : byte
    {
        Enabled,
        Disabled,
    }

    public static void SendSkinEnabled()
    {
        var writer = new FastBufferWriter(8, Allocator.Temp, 1200);
        writer.WriteValueSafe(true);
        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(
            "SkinStateUpdate",
            NetworkManager.ServerClientId,
            writer,
            NetworkDelivery.Reliable
        );
    }

    public static void SendSkinDisabled()
    {
        var writer = new FastBufferWriter(8, Allocator.Temp, 1200);
        writer.WriteValueSafe(false);
        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(
            "SkinStateUpdate",
            NetworkManager.ServerClientId,
            writer,
            NetworkDelivery.Reliable
        );
    }

    public static void RegisterServerRecievers()
    {
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(
            "SkinUpdate",
            (ulong clientId, FastBufferReader reader) =>
            {
                if (
                    ChangeSkinMonoBehaviour.initialized
                    && !ChangeSkinMonoBehaviour.replacers[clientId].isBanned
                )
                {
                    {
                        reader.ReadValueSafe(out string skin);
                        reader.ReadValueSafe(out string url);
                        if (clientId != NetworkManager.ServerClientId)
                        {
                            ChangeSkinMonoBehaviour.replacers[clientId].LoadSkinURL(url, skin);
                        }
                        var writer = new FastBufferWriter(8, Allocator.Temp, 1200);
                        writer.WriteValueSafe(clientId);
                        writer.WriteValueSafe(skin);
                        writer.WriteValueSafe(url);
                        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(
                            "PlayerSkinRelay",
                            ScavMultiGlobalSynchronizer.AllClientIdsExceptHost,
                            writer,
                            NetworkDelivery.Reliable
                        );
                        writer.Dispose();
                        if (Plugin.ModConfig.Verbose)
                        {
                            Plugin.Logger.LogInfo($"SkinUpdate: {skin} + {url} from {clientId}");
                        }
                    }
                }
            }
        );
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(
            "SkinStateUpdate",
            (ulong clientId, FastBufferReader reader) =>
            {
                if (
                    ChangeSkinMonoBehaviour.initialized
                    && !ChangeSkinMonoBehaviour.replacers[clientId].isBanned
                )
                {
                    reader.ReadValueSafe(out bool state);
                    if (clientId != NetworkManager.ServerClientId)
                    {
                        if (state)
                        {
                            ChangeSkinMonoBehaviour.replacers[clientId].BeginReplacement();
                        }
                        if (!state)
                        {
                            ChangeSkinMonoBehaviour.replacers[clientId].StopReplacement();
                        }
                    }
                    var writer = new FastBufferWriter(8, Allocator.Temp, 1200);
                    writer.WriteValueSafe(clientId);
                    writer.WriteValueSafe(state);
                    NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(
                        "SkinStateUpdateRelay",
                        ScavMultiGlobalSynchronizer.AllClientIdsExceptHost,
                        writer,
                        NetworkDelivery.Reliable
                    );
                    writer.Dispose();
                    if (Plugin.ModConfig.Verbose)
                    {
                        Plugin.Logger.LogInfo($"SkinStateUpdate: {state} from {clientId}");
                    }
                }
            }
        );
    }

    public static void RegisterClientRecievers()
    {
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(
            "PlayerSkinRelay",
            (ulong clientId, FastBufferReader reader) =>
            {
                if (clientId != NetworkManager.ServerClientId)
                    return;
                if (ChangeSkinMonoBehaviour.initialized
                // && clientId != NetworkManager.ServerClientId
                )
                {
                    reader.ReadValueSafe(out ulong recivedClientId);
                    if (!ChangeSkinMonoBehaviour.replacers[recivedClientId].isBanned
                    // && recivedClientId != ChangeSkinMonoBehaviour.localPlayerBody.clientId
                    )
                    {
                        reader.ReadValueSafe(out string skin);
                        reader.ReadValueSafe(out string url);
                        ChangeSkinMonoBehaviour.replacers[recivedClientId].LoadSkinURL(url, skin);
                        if (Plugin.ModConfig.Verbose)
                        {
                            Plugin.Logger.LogInfo($"PlayerSkinRelay recieved: {skin} + {url}");
                        }
                    }
                }
            }
        );
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(
            "SkinStateUpdateRelay",
            (ulong clientId, FastBufferReader reader) =>
            {
                if (ChangeSkinMonoBehaviour.initialized
                // && clientId != NetworkManager.ServerClientId
                )
                {
                    reader.ReadValueSafe(out ulong recivedClientId);
                    reader.ReadValueSafe(out bool state);
                    if (
                        !ChangeSkinMonoBehaviour.replacers[recivedClientId].isBanned
                        && recivedClientId != ChangeSkinMonoBehaviour.localPlayerBody.clientId
                    )
                    {
                        if (state)
                        {
                            ChangeSkinMonoBehaviour.replacers[recivedClientId].BeginReplacement();
                        }
                        if (!state)
                        {
                            ChangeSkinMonoBehaviour.replacers[recivedClientId].StopReplacement();
                        }
                        if (Plugin.ModConfig.Verbose)
                        {
                            Plugin.Logger.LogInfo(
                                $"SkinStateUpdateRelay recieved: {recivedClientId} + {state}"
                            );
                        }
                    }
                }
            }
        );
    }

    // private static void Server_UploadLocalSkinToURL(ulong clientId, ref FastBufferReader reader)
    // {
    //     // FastBufferWriter fastBufferWriter = new FastBufferWriter(256, Allocator.Temp);
    //     // fastBufferWriter.WriteValueSafe(keyValuePair.Key);
    //     // fastBufferWriter.WriteValueSafe(keyValuePair.Value.name);
    //     // NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(
    //     //     "ChangeSkinClientSkinUpdate",
    //     //     clientId,
    //     //     fastBufferWriter
    //     // );
    //     // fastBufferWriter.Dispose();
    // }

    // [ClientKrokoshaReciever("ChangeSkinClientSkinUpdate")]
    // private static void Client_ChangeSkinClientSkinUpdate(ulong _, ref FastBufferReader reader)
    // {
    //     if (ChangeSkinMonoBehaviour.initialized)
    //     {
    //         ulong id;
    //         string url;
    //         string skinName;
    //         reader.ReadValueSafe(out id);
    //         reader.ReadValueSafe(out url);
    //         reader.ReadValueSafe(out skinName);
    //         ChangeSkinMonoBehaviour.replacers[id].LoadSkinURL(url, skinName);
    //     }
    // }

    // [ServerKrokoshaReciever("ChangeSkinLocalSkinSend")]
    // private static void Server_LocalSkinReciever(ulong clientId, ref FastBufferReader reader)
    // {
    //     if (ChangeSkinMonoBehaviour.initialized)
    //     {
    //         string url;
    //         string skinName;
    //         reader.ReadValueSafe(out url);
    //         reader.ReadValueSafe(out skinName);
    //         SkinLoader.DownloadRemote(url);
    //         ChangeSkinMonoBehaviour.replacers[clientId].LoadSkinURL(url, skinName);
    //     }
    // }

    // [ClientKrokoshaReciever("ChangeSkinLocalSkinRequest")]
    // private static void Client_LocalSkinRequest(ulong _, ref FastBufferReader reader)
    // {
    //     if (ChangeSkinMonoBehaviour.initialized)
    //     {
    //         // KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer(
    //         //     "ChangeSkinLocalSkinSend",
    //         //     ChangeSkinMonoBehaviour.localSkin
    //         // );
    //     }
    // }
}
