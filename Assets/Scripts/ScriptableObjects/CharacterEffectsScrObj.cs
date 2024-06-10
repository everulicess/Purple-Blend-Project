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
    public AudioClip[] Attack_1_Sounds;
    [HideInInspector]
    public AudioClip Attack_1_Sound
    {
        get
        {
            int pos = Random.Range(0, Attack_1_Sounds.Length);
            return Attack_1_Sounds[pos];
        }
    }
    public VisualEffect Attack_1_Visual;

    [HideInInspector]
    public AudioClip Attack_2_Sound
    {
        get
        {
            int pos = Random.Range(0, Attack_2_Sounds.Length);
            return Attack_2_Sounds[pos];
        }
    }
    public AudioClip[] Attack_2_Sounds;
    public VisualEffect Attack_2_Visual;

    [HideInInspector]
    public AudioClip Attack_3_Sound
    {
        get
        {
            int pos = Random.Range(0, Attack_3_Sounds.Length);
            return Attack_3_Sounds[pos];
        }
    }
    public AudioClip[] Attack_3_Sounds;
    public VisualEffect Attack_3_Visual;

    [Header("Special Attack Effects")]
    public AudioClip Special_Attack_Sound;
    public VisualEffect Special_Attack_Visual;

    [Header("Damaged Effects")]
    public AudioClip[] Damaged_Sounds;
    [HideInInspector]
    public AudioClip Damaged_Sound 
    {
        get
        {
            return Damaged_Sounds[Random.Range(0, Damaged_Sounds.Length)];
        }
    }
    public AudioClip Death_Sound;
}
