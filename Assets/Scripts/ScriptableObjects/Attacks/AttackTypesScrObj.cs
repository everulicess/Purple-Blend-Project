using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Scriptable Objects/Attack Types")]
public class AttackTypesScrObj : ScriptableObject
{
    public float damage;
    public float knockback;
    public Mesh colliderShape;
}
