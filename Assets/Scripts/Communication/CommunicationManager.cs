using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public enum Pings
{
    None,
    Attack,
    Loot,
    GreatJob,
    Thanks,
    Help,
    NeedToDeposit,
}
[Serializable]
public class PingInfo
{
    [Tooltip("3D visual for the ping")] 
    public NetworkObject Prefab;

    [Tooltip("It will be played when the ping is placed")] 
    public AudioClip Sound;

    [Tooltip("Icon that will be displayed in the Menu")] 
    public Texture Icon;
}
[Serializable]
public struct CommunicationLibrary
{
    [Header("Pings")]
    public PingInfo Attack_Ping;
    public PingInfo Loot_Ping;
    public PingInfo GreatJob_Ping;
    public PingInfo Thanks_Ping;
    public PingInfo Help_Ping;
    public PingInfo NeedToDeposit_Ping;
}
public class CommunicationManager : MonoBehaviour
{
    [SerializeField] CommunicationLibrary communicationLibrary;

    public static Dictionary<Pings, AudioClip> audioDictionary = new() { };
    public static Dictionary<Pings, NetworkObject> visualsDictionary = new();
    public static Dictionary<Pings, Texture> IconsDictionary = new();

    private void Awake()
    {
        InitializeDictionaries();
    }
    private void InitializeDictionaries()
    {
        //Sounds
        audioDictionary.Add(Pings.Attack, communicationLibrary.Attack_Ping.Sound);
        audioDictionary.Add(Pings.Loot, communicationLibrary.Loot_Ping.Sound);
        audioDictionary.Add(Pings.GreatJob, communicationLibrary.GreatJob_Ping.Sound);
        audioDictionary.Add(Pings.Thanks, communicationLibrary.Thanks_Ping.Sound);
        audioDictionary.Add(Pings.Help, communicationLibrary.Help_Ping.Sound);
        audioDictionary.Add(Pings.NeedToDeposit, communicationLibrary.NeedToDeposit_Ping.Sound);

        //Visuals
        visualsDictionary.Add(Pings.Attack, communicationLibrary.Attack_Ping.Prefab);
        visualsDictionary.Add(Pings.Loot, communicationLibrary.Loot_Ping.Prefab);
        visualsDictionary.Add(Pings.GreatJob, communicationLibrary.GreatJob_Ping.Prefab);
        visualsDictionary.Add(Pings.Thanks, communicationLibrary.Thanks_Ping.Prefab);
        visualsDictionary.Add(Pings.Help, communicationLibrary.Help_Ping.Prefab);
        visualsDictionary.Add(Pings.NeedToDeposit, communicationLibrary.NeedToDeposit_Ping.Prefab);

        //Icons
        IconsDictionary.Add(Pings.Attack, communicationLibrary.Attack_Ping.Icon);
        IconsDictionary.Add(Pings.Loot, communicationLibrary.Loot_Ping.Icon);
        IconsDictionary.Add(Pings.GreatJob, communicationLibrary.GreatJob_Ping.Icon);
        IconsDictionary.Add(Pings.Thanks, communicationLibrary.Thanks_Ping.Icon);
        IconsDictionary.Add(Pings.Help, communicationLibrary.Help_Ping.Icon);
        IconsDictionary.Add(Pings.NeedToDeposit, communicationLibrary.NeedToDeposit_Ping.Icon);
    }
}


