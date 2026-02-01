// using System;
// using KrokoshaCasualtiesMP;
// using Unity.Netcode;
// using UnityEngine;

// namespace ChangeSkin;

// public class ChangeSkinNetworkComponent : MonoBehaviour
// {
//     private static bool server_has_changeskin = false;

//     private void Start()
//     {
//         if(KrokoshaScavMultiplayer.is_client)

//         TimeScaleIndependentInvokeRepeatingStatic.AddFunc(
//             new Action(SkinSync),
//             0.4067f,
//             5f,
//             false,
//             "FluidTilemapSyncUpdate",
//             false
//         );
//     }

//     private static void SkinSync()
//     {

//     }



//     [ServerKrokoshaReciever("ChangeSkinServerFirstCall")]
//     private static void ChangeSkinServerFirstCall(ulong clientId, ref FastBufferReader reader) 
//     {
//         ScavClientInstance scavClientInstance;
//         Body body;
//         if (ScavClientInstance.TryGetScavClientInstanceAndBodyFromClientId(clientId, out scavClientInstance, out body))
//         {
//             body.gameObject.AddComponent<ChangeBody>();
//             KrokoshaScavMultiplayer.Server_SendRelayMessageToClients("CurrentSkinRequest", clientId);
//         }
//     }

//     [ClientKrokoshaReciever]
// }
