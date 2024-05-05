using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

public class PingHandler : NetworkBehaviour
{
    float destroyingTime;
    NetworkObject ThisObject;

    private void Awake()
    {
        Init();
    }
    public override void Spawned()
    {
        ThisObject = GetComponent<NetworkObject>();
    }
    public void Init()
    {
        destroyingTime = 1.5f;
    }
    public override void FixedUpdateNetwork()
    {
        if (destroyingTime < 0)
        {
            Runner.Despawn(ThisObject);
        }
    }
    void Update()
    {
        destroyingTime -= Runner.DeltaTime;
    }
}
