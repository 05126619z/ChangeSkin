using System;
using System.Collections.Generic;
using KrokoshaCasualtiesMP;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace ChangeSkin;

public class ChangeSkinNetworkComponent : MonoBehaviour
{
    [ServerKrokoshaReciever("ChangeSkinInitMessage")]
    private static void Server_ChangeSkinInitMessage(ulong clientId, ref FastBufferReader reader)
    {
        if (ChangeSkinMonoBehaviour.initialized)
        {
            {
                string skin;
                reader.ReadValueSafe(out skin, false);
                foreach (
                    KeyValuePair<
                        ulong,
                        ChangeBody
                    > keyValuePair in ChangeSkinMonoBehaviour.replacers
                )
                {
                    FastBufferWriter fastBufferWriter = new FastBufferWriter(256, Allocator.Temp);
                    fastBufferWriter.WriteValueSafe(keyValuePair.Key);
                    fastBufferWriter.WriteValueSafe(keyValuePair.Value.name);
                    NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(
                        "ChangeSkinClientSkinUpdate",
                        clientId,
                        fastBufferWriter
                    );
                    fastBufferWriter.Dispose();
                }
            }
        }
    }

    [ClientKrokoshaReciever("ChangeSkinClientSkinUpdate")]
    private static void Client_ChangeSkinClientSkinUpdate(ulong _, ref FastBufferReader reader)
    {
        if (ChangeSkinMonoBehaviour.initialized)
        {
            ulong id;
            string url;
            string skinName;
            reader.ReadValueSafe(out id);
            reader.ReadValueSafe(out url);
            reader.ReadValueSafe(out skinName);
            ChangeSkinMonoBehaviour.replacers[id].LoadSkinURL(url, skinName);
        }
    }

    [ServerKrokoshaReciever("ChangeSkinLocalSkinSend")]
    private static void Server_LocalSkinReciever(ulong clientId, ref FastBufferReader reader)
    {
        if (ChangeSkinMonoBehaviour.initialized)
        {
            string url;
            string skinName;
            reader.ReadValueSafe(out url);
            reader.ReadValueSafe(out skinName);
            SkinLoader.DownloadRemote(url);
            ChangeSkinMonoBehaviour.replacers[clientId].LoadSkinURL(url, skinName);
        }
    }

    [ClientKrokoshaReciever("ChangeSkinLocalSkinRequest")]
    private static void Client_LocalSkinRequest(ulong _, ref FastBufferReader reader)
    {
        if (ChangeSkinMonoBehaviour.initialized)
        {
            // KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer(
            //     "ChangeSkinLocalSkinSend",
            //     ChangeSkinMonoBehaviour.localSkin
            // );
        }
    }
}
