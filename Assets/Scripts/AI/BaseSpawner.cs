using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class BaseSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkObject enemy;
    private NetworkObject spawnedEnemy;

    // Start is called before the first frame update

    public void SpawnEnemy()
    {
        spawnedEnemy = Runner.Spawn(enemy, transform.position);
    }
}
