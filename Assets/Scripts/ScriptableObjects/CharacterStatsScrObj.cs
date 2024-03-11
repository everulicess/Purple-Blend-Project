using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Characters
{
    TheMule,
    TheBoomstick,
    TheSiren
}
[Serializable]
public struct CharacterMovementStats
{
    public Characters CharacterName;
    public float MovementSpeed;
}
[Serializable]
public struct CharacterCombatStats
{
    public float Damage;
    public float Penetration;
    public float Range;
    public Animator Anim;
}
[CreateAssetMenu(menuName = "Scriptable Objects/Character Info")]
public class CharacterStatsScrObj : ScriptableObject
{
    public CharacterMovementStats MovementStats;
}
