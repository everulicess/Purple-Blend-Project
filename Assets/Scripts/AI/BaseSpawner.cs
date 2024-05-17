using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.AI.Navigation;
using System;

public class BaseSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkObject enemy;
    private NetworkObject spawnedEnemy;

    public void SpawnEnemy()
    {
        spawnedEnemy = Runner.Spawn(enemy, transform.position);
    }
}
