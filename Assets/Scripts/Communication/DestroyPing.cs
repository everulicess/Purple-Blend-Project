using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class DestroyPing : NetworkBehaviour
{
    [SerializeField] float destroyingTime;
    [SerializeField] NetworkObject ThisObject;
    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        if (destroyingTime < 0)
        {
            Runner.Despawn(ThisObject);
            //Destroy(this.gameObject);
        }
    }
    void Update()
    {
        destroyingTime -= Time.deltaTime;

    }
}
