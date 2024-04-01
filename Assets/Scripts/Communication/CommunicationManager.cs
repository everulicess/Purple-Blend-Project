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
    [SerializeField] CommunicationLibrary communicationLibrary;

    public static Dictionary<Pings, AudioClip> audioDictionary = new() { };
    public static Dictionary<Pings, NetworkObject> visualsDictionary = new();
    public static Dictionary<Pings, Texture> IconsDictionary = new();

    public static Vector3 PingMenuPosition;

    [SerializeField] RadialMenu radialMenu;
    private Camera cam;
   
    private void Awake()
    {
        radialMenu = FindObjectOfType<RadialMenu>();
        InitializeDictionaries();
    }
    private void Start()
    {
        cam = FindObjectOfType<LocalCamera>().GetComponentInChildren<Camera>();
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
            
             PingMenuPosition = Input.mousePosition;
           
        }
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


