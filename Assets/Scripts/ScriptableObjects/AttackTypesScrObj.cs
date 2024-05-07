using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Scriptable Objects/Attack Types")]
public class AttackTypesScrObj : ScriptableObject
{
    public byte damage;
    public float knockback;
    public float playerPush;
    public Mesh colliderShape;
}
