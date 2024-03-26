using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public enum Pings
{
    None,
    MissingPing,
    LocationPing,
    NewPing,
    AnotherPing
}
[Serializable]
public class PingInfo
{
    [Tooltip("3D visual for the ping")] public NetworkObject Prefab;
    [Tooltip("It will be played when the ping is placed")] public AudioClip Sound;
    [Tooltip("Icon that will be displayed in the Menu")] public Texture Icon;
}
[Serializable]
public struct CommunicationLibrary
{
    [Header("Pings")]
    public PingInfo Ping1;
    public PingInfo Ping2;
    public PingInfo Ping3;
    public PingInfo Ping4;
    public PingInfo Ping5;
}
public class CommunicationManager : NetworkBehaviour
{
    [SerializeField]CommunicationLibrary communicationLibrary;

    private static Dictionary<Pings, AudioClip> audioDictionary = new() { };
    private static Dictionary<Pings, NetworkObject> visualsDictionary = new();
    public static Dictionary<Pings, Texture> IconsDictionary = new();

    public static Vector3 PingMenuPosition;

    [SerializeField] RadialMenu radialMenu;
    //[Networked(),OnChangedRender(nameof(OnPingPlaced))]
    //bool IsMouseOverGameWindow { 
    //    get { 
    //        return !(0 > Input.mousePosition.x || 0 > Input.mousePosition.y || Screen.width < Input.mousePosition.x || Screen.height < Input.mousePosition.y); } }
    //void OnPingPlaced()
    //{

    //}
    public override void Spawned()
    {
        //InitializeDictionaries();
        //MenuToggle();
    }
    private void Awake()
    {

        InitializeDictionaries();
    }
    public override void FixedUpdateNetwork()
    {
        //MenuToggle();
    }
    void Update()
    {
        MenuToggle();
    }
    //NetworkInput input;
    private void MenuToggle()
    {
        if (!Runner) return;
       
        PlayerRef player = Runner.LocalPlayer;
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.V))
        {
            radialMenu.Open();
            radialMenu.transform.position = PingMenuPosition;
            //Debug.LogWarning($"spawning menu vector: {radialMenu.transform.position}");
            //Cursor.visible = false;
        }
        else
        {
            radialMenu.Close();
            Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
            if (player == Runner.LocalPlayer && Input.mousePresent) 
            {

                //Debug.LogError($"this player is: {player.GetType().FullName}");

                //GetInput(out NetworkInputData data);
                //if (data.direction!=null)
                data.MousePosition = Input.mousePosition;
                //input.Set(data);
                //{
                PingMenuPosition = data.MousePosition;

                //Debug.LogWarning($"my local mouse position is {PingMenuPosition}");
                //}
                    

            }
            //else
            //{
            //    Debug.LogWarning($"other player's mouse position is {Input.mousePosition}");
            //}
            //GetLocalAuthorityMask();
            //Debug.LogError($"local authority mask: {GetLocalAuthorityMask()}");
            //pingMenuUIObject.SetActive(false);
        }
    }

    //[Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsHostPlayer)]
    [Rpc(RpcSources.All, RpcTargets.StateAuthority, InvokeLocal = true, Channel = RpcChannel.Reliable)]
    public void PlacePing_RPC(RadialMenuEntry pEntry,Pings pPingId)
    {
        Vector3 pMousePosition = Input.mousePosition;
        //if (!Object.HasInputAuthority) return;

        //if (Runner.IsClient || HasInputAuthority || HasStateAuthority)
        //{
        //if (pEntry == null) return;
        
        //Debug.LogError($"can send the RPC \n PingID being placed ====== {pEntry.GetPingID()}");

        //Pings _ping = pEntry != null ? pEntry.GetPingID() : Pings.NewPing;
        Pings _ping =pPingId;

            //get sound and visuals for each ping
            visualsDictionary.TryGetValue(_ping, out NetworkObject _pingVisual);
            audioDictionary.TryGetValue(_ping, out AudioClip _pingSound);

        Vector3 pos = MousePosition._rect.position;
        Camera cam = CameraFollow.Singleton.GetComponent<Camera>();
        //Vector3 posWorld = cam.ScreenToWorldPoint(Input.mousePosition);

        Ray ray = cam.ScreenPointToRay(pos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject.layer == 8)
                {
                    Vector3 offset = new(hit.point.x, hit.point.y + 0.1f, hit.point.z);
                //if (HasStateAuthority)
                //{
                    Runner.Spawn(_pingVisual, offset, Quaternion.identity, Runner.LocalPlayer,
                        (Runner, O) =>
                        {
                            O.GetComponent<DestroyPing>().Init();
                        }
                        );
                //}
                AudioSource.PlayClipAtPoint(_pingSound, offset);
                Debug.Log($"{_pingVisual.name} has been placed and {_pingSound.name} is being played");
                }
            }
        //}
        //else
        //{
        //    Debug.LogError($"not able to send the RPC, input authority: {HasInputAuthority}");
        //}
        //if (runner) return;

    }
    private void InitializeDictionaries()
    {
        //Sound
        audioDictionary.Add(Pings.MissingPing, communicationLibrary.Ping1.Sound);
        audioDictionary.Add(Pings.LocationPing, communicationLibrary.Ping2.Sound);
        audioDictionary.Add(Pings.None, communicationLibrary.Ping3.Sound);
        audioDictionary.Add(Pings.AnotherPing, communicationLibrary.Ping4.Sound);
        audioDictionary.Add(Pings.NewPing, communicationLibrary.Ping5.Sound);

        //Visual
        visualsDictionary.Add(Pings.MissingPing, communicationLibrary.Ping1.Prefab);
        visualsDictionary.Add(Pings.LocationPing, communicationLibrary.Ping2.Prefab);
        visualsDictionary.Add(Pings.None, communicationLibrary.Ping3.Prefab);
        visualsDictionary.Add(Pings.AnotherPing, communicationLibrary.Ping4.Prefab);
        visualsDictionary.Add(Pings.NewPing, communicationLibrary.Ping5.Prefab);

        //Icons
        IconsDictionary.Add(Pings.MissingPing, communicationLibrary.Ping1.Icon);
        IconsDictionary.Add(Pings.LocationPing, communicationLibrary.Ping2.Icon);
        IconsDictionary.Add(Pings.None, communicationLibrary.Ping3.Icon);
        IconsDictionary.Add(Pings.AnotherPing, communicationLibrary.Ping4.Icon);
        IconsDictionary.Add(Pings.NewPing, communicationLibrary.Ping5.Icon);
    }
}


