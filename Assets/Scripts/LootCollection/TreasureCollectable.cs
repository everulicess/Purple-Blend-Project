using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TreasureCollectable : Collectable
{
    public override void Spawned()
    {
        net_ThisObject = this.GetComponent<NetworkObject>();
    }
    public override void TryInteracting(Collector p_Collector)
    {
        base.TryInteracting(p_Collector);
        if (playerCollector.CanPickUp())
        {
            p_Collector.SetObjectToPickUp(net_ThisObject);
        }
        Interact();
    }
    public override void Interact()
    {
        if (!playerCollector) return;
        playerCollector.CollectTreasure(this, this.GetComponent<Rigidbody>(), this.gameObject.GetComponent<BoxCollider>());
        
    }
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        playerCollector.SetInteractionBool(false);
        base.Despawned(runner, hasState);
    }
    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent<Deposit>(out Deposit _deposit);
        if (_deposit != null)
        {
            _deposit.UpdateGlobalGold(goldValue);
            DeleteObject();
        }
    }
}
