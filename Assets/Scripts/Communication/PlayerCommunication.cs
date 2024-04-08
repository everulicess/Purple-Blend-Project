using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerCommunication : NetworkBehaviour
{
    
    [SerializeField] NetworkObject debugObject;
    //bool placePing;

    //[Header("Ping Info")]
    NetworkObject pingVisual;
    Pings pingID;
    private void Start()
    {
        //Debug.LogWarning($"this object {nameof(PlayerCommunication)} is in the scene");
    }
    private void Update()
    {
        //if (!placePing) return;
        if (Input.GetKeyUp(KeyCode.V) /*&& HasInputAuthority*/)
        {
            RPC_SendMessage( MousePosition.InWorldRayPosition);
            //pingID = SetPingToDisplay(pingID);
            Debug.LogError($"Ping that will be passed to the object: {pingID}");
        }
        
    }
    public void SetPingToDisplay(Pings pPing)
    {
        pingID = pPing;
        //Debug.LogWarning($"{pingID} is assigned {pPing}");
        //return pingID;
        //placePing = true;
    }
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SendMessage( Vector3 pVector, RpcInfo info = default)
    {
        
        RPC_RelayMessage( pVector, info.Source);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_RelayMessage( Vector3 pVector,PlayerRef messageSource)
    {
        string message = "";
        
        if (messageSource == Runner.LocalPlayer)
        {
            message = $"You: vector = {pVector}\n PingID = {pingID}";
        }
        else
        {
            message = $"other player: vector = {pVector}\n PingID = {pingID}";
        }
        Debug.LogWarning(message);

        NetworkObject ping = Runner.Spawn(debugObject, pVector, Quaternion.identity);
        ping.GetComponent<DestroyPing>().SetPing(pingID);

    }
}
