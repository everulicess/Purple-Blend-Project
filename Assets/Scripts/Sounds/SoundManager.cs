using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fusion;

public class SoundManager : MonoBehaviour
{
    [SerializeField] Sound[] sounds;
    static readonly Dictionary<SoundName, AudioClip> SoundDictionary = new();
    static NetworkRunner _Runner;
    public static SoundManager instance;
    AudioSource audioSource;
   
    private void Start()
    {
        audioSource=this.GetComponent<AudioSource>();
        SoundDictionary.Clear();
        foreach (Sound s in sounds)
        {
            SoundDictionary.Add(s.Name, s.SoundToPlay);
        }
        instance = this;
    }
    private void Update()
    {
        if (_Runner==null)
        {
            _Runner = FindObjectOfType<NetworkRunner>();
        }
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority, Channel = RpcChannel.Unreliable)]
    public static void RPC_PlaySound(SoundName pName, Vector3 pPosition, RpcInfo info = default)
    {
        RPC_ServerToClient_PlaySound(pName, pPosition, info.Source);
    }
    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All, Channel = RpcChannel.Unreliable)]
    public static void RPC_ServerToClient_PlaySound(SoundName pName, Vector3 pPosition, PlayerRef messageSource)
    {
        //Debug.LogError($"sound name {pName}, at position {pPosition}");

        if (SoundDictionary.TryGetValue(pName, out AudioClip sound))
        {
            if (messageSource == _Runner.LocalPlayer)
            {
                AudioSource.PlayClipAtPoint(sound, pPosition);
            }
            else
            {
                AudioSource.PlayClipAtPoint(sound, pPosition);
            }
        }

    }
}

[Serializable]
public class Sound
{
    public SoundName Name;
    public AudioClip SoundToPlay;
}

public enum SoundName
{
    MuleAttack1,
    MuleAttack2,
    MuleAttack3,
    MuleStep,
    MuleDeath,
    MuleHurt,
    SirenHurt,
    SirenAttack,
    SirenDeath,
    SirenStep,
    BoomstickHurt,
    BoomstickDeath,
    BoomstickFlintlock,
    BoomstickStep,
    BoomstickSword,
    GruntAttack1,
    GruntAttack2,
    GruntAttack3,
    GruntDeath,
    GruntHurt1,
    GruntHurt2,
    GruntHurt3,
    GruntStep,
    JuggernautAttack,
    JuggernautHurt,
    JuggernautDeath,
    JuggernautStep,
    RangedGruntAttack,
    Ambient,
    Drip,
    CoinsPickup,
    RelicPickup,
    LootDelivered
}
