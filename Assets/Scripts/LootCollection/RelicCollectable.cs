using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class RelicCollectable : Collectable
{
    public override void Spawned()
    {
        net_ThisObject = this.gameObject.GetComponent<NetworkObject>();
    }
    public override void TryInteracting(Collector p_Collector)
    {
        base.TryInteracting(p_Collector);
        Interact();

    }
    public override void Interact()
    {
        playerCollector.CollectRelic(this, goldValue);
    }
}
