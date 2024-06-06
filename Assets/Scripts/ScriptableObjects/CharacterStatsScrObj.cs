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
    public float Range;
    public bool isRanged;
    public bool Special;
    public float comboMaxTime;
    public List<AttackTypesScrObj> attackTypes;
    public AttackTypesScrObj specialType;
}
[CreateAssetMenu(menuName = "Scriptable Objects/Character Info")]
public class CharacterStatsScrObj : ScriptableObject
{
    [Header("Movement Data Setting")]
    public CharacterMovementStats MovementStats;

    [Header("Combat Data Setting")]
    public CharacterCombatStats CombatStats;
}
