using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerCommunication : NetworkBehaviour
{
    Pings pingID { get; set; }

    Player m_Player;
    [SerializeField] RadialMenu radialMenu;
    Vector3 PingMenuPosition;

    private void Start()
    {
        if (Player.Local == null)
            return;
        m_Player = GetComponent<Player>();
    }
    public override void FixedUpdateNetwork()
    {
        if (Player.Local == null)
            return;
        if (!m_Player.Object.HasInputAuthority)
            return;

        if (!GetInput(out NetworkInputData data)) 
            return;
        //Ping Menu open and close
        MenuToggle(data);

        //place chosen Ping in game world
        if (data.buttons.IsSet(MyButtons.PingsButtonReleased))
                RPC_PlacePingClientToServer(pingID, data.pointToLookAt);
    }
    private void MenuToggle(NetworkInputData data)
    {
        if (Input.GetKey(KeyCode.V))
        {
            radialMenu.Open();
            radialMenu.transform.position = PingMenuPosition;
        }
        else
        {
            radialMenu.Close();
            PingMenuPosition = Input.mousePosition;
        }
    }

    //called from the menu entries to assign the ping to be placed
    public void SetPingToDisplay(Pings pPing)
    {
        //check if it's a player
        if (!m_Player.Object.HasInputAuthority) 
            return;

        pingID = pPing;
    }
    //remote procedural call sent from the client to the server
    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority/*, HostMode = RpcHostMode.SourceIsHostPlayer*/)]
    public void RPC_PlacePingClientToServer(Pings pPingID ,Vector3 pVector, RpcInfo info = default)
    {
        if (pPingID == Pings.None) 
            return;

        //calls the server to client rpc
        RPC_PlacePingServerToClients( pVector, pPingID, info.Source);
    }

    //remote procediral call sent from the server to the client, synchronizes everyone
    [Rpc(RpcSources.StateAuthority, RpcTargets.All/*, HostMode = RpcHostMode.SourceIsServer*/)]
    public void RPC_PlacePingServerToClients( Vector3 pVector, Pings pPingID, PlayerRef messageSource)
    {
        //gets the ping's visual
        CommunicationManager.visualsDictionary.TryGetValue(pPingID, out NetworkObject visual);

        //gets the ping's audio
        CommunicationManager.audioDictionary.TryGetValue(pPingID, out AudioClip sound);
        
        //Spawning the ping
        Runner.Spawn(visual, pVector, Quaternion.identity);

        //playing the sound
        AudioSource.PlayClipAtPoint(sound, pVector);
    }
}
