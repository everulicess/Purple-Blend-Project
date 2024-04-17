using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerCommunication : NetworkBehaviour
{
    
     NetworkObject debugObject;
    //bool placePing;
    bool openMenu;
    //[Header("Ping Info")]
    NetworkObject pingVisual;
    //Runner reference
    //NetworkRunner Runner;
    Pings pingID { get; set; }
    private void Start()
    {
        //Runner = FindObjectOfType<NetworkRunner>();
    }
    private void Update()
    {

    }
    public void SetPingMenuInteraction(bool _value)
    {
        openMenu = _value;
    }
    public void SetPingToDisplay(Pings pPing)
    {
        pingID = pPing;
        RPC_SendMessage(pPing, MousePosition.InWorldRayPosition);
        Debug.Log($"{pingID} is assigned {pPing}");
    }
    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SendMessage(Pings pPingID ,Vector3 pVector, RpcInfo info = default)
    {
        CommunicationManager.visualsDictionary.TryGetValue(pPingID, out NetworkObject visual);
        RPC_RelayMessage( pVector, visual, info.Source);
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_RelayMessage( Vector3 pVector,NetworkObject pVisual ,PlayerRef messageSource)
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
        

        NetworkObject ping = Runner.Spawn(pVisual, pVector, Quaternion.identity);
        ping.GetComponent<DestroyPing>().SetPing(pingID);
    }
}
