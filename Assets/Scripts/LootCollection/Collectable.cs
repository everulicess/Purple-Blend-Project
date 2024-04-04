using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class Collectable : NetworkBehaviour
{
    public NetworkObject net_ThisObject;
    protected Collector playerCollector;
    [SerializeField] protected float goldValue;
    public virtual void TryInteracting(Collector p_Collector)
    {
        playerCollector = p_Collector;
    }
    public virtual void Interact()
    {
        //has to be filled for each subclass
    }
    public virtual void DeleteObject()
    {
        Runner.Despawn(net_ThisObject);
    }
}
