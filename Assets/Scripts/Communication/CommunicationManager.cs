using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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
    [Tooltip("3D visual for the ping")] public GameObject Prefab;
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
public class CommunicationManager : MonoBehaviour
{
    [SerializeField]CommunicationLibrary communicationLibrary;

    public static Dictionary<Pings, AudioClip> audioDictionary = new();
    public static Dictionary<Pings, GameObject> visualsDictionary = new();
    public static Dictionary<Pings, Texture> IconsDictionary = new();

    public static Vector3 pingMenuPosition { private set; get; }

    [SerializeField] RadialMenu radialMenu;
    private void Awake()
    {
        InitializeDictionaries();
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.V))
        {
            radialMenu.Open();
            radialMenu.transform.position = pingMenuPosition;
            //Cursor.visible = false;
        }
        else
        {
            radialMenu.Close();
            Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
            pingMenuPosition = Input.mousePosition;
            //pingMenuUIObject.SetActive(false);
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


