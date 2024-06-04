using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fusion;

public class CoinsCollectable : Collectable
{
    public override void Spawned()
    {
        net_ThisObject = this.GetComponent<NetworkObject>();
    }
    public override void TryInteracting(Collector p_Collector)
    {
        base.TryInteracting(p_Collector);
        Interact();
    }
    public override void Interact()
    {
        playerCollector.CollectCoins(this, goldValue);

    }
}
