using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "EffectsData", menuName = "Scriptable Objects/EffectsData")]
public class CharacterEffectsScrObj : ScriptableObject
{
    [Header("Movement Effects")]
    public AudioClip Footsteps_Sound;

    [Header("Attack Effects")]
    [SerializeField]
    AudioClip[] Attack_1_Sounds;
    [HideInInspector]
    public AudioClip Attack_1_Sound
    {
        get
        {
            int pos = Random.Range(0, Attack_1_Sounds.Length);
            return Attack_1_Sounds[pos];
        }
    }
    //public VisualEffectAsset Attack_1_Visual;

    [HideInInspector]
    public AudioClip Attack_2_Sound
    {
        get
        {
            int pos = Random.Range(0, Attack_2_Sounds.Length);
            return Attack_2_Sounds[pos];
        }
    }
    [SerializeField]
    AudioClip[] Attack_2_Sounds;
    //public VisualEffectAsset Attack_2_Visual;

    [HideInInspector]
    public AudioClip Attack_3_Sound
    {
        get
        {
            int pos = Random.Range(0, Attack_3_Sounds.Length);
            return Attack_3_Sounds[pos];
        }
    }
    [SerializeField]
    AudioClip[] Attack_3_Sounds;
    //public VisualEffectAsset Attack_3_Visual;

    [Header("Special Attack Effects")]
    public AudioClip Special_Attack_Sound;
    //public VisualEffectAsset Special_Attack_Visual;

    [Header("Damaged Effects")]
    [SerializeField] 
    AudioClip[] Damaged_Sounds;
    [HideInInspector]
    public AudioClip Damaged_Sound 
    {
        get
        {
            return Damaged_Sounds[Random.Range(0, Damaged_Sounds.Length)];
        }
    }
    public AudioClip Death_Sound;

    //public VisualEffectAsset vfx;
}
