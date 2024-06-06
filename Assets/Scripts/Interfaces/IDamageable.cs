using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public interface IDamageable
{
    /// <summary>
    /// Call when dealing damage, need to call the "Damage_ToHostRPC" to communicate the action to everyone.
    /// Call it when dealing damage
    /// </summary>
    void OnTakeDamage(float pDamage);
    /// <summary>
    /// RPC that should be called when dealing damage, holds the call to "Damage_toClientsRPC"
    /// RPC send to the server, needs to use the fusion namespace, and have 
    /// [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)] on top of the void
    /// </summary>
    //void Damage_ToHostRPC(float pDamage);
    /// <summary>
    /// RPC that should be called inside the "Damage_ToHostRPC", holds the logic for taking damage
    /// RPC send to the clients, needs to use the fusion namespace, and have 
    /// [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)] on top of the void
    /// </summary>
    //void Damage_ToClientsRPC(float pDamage);
}
