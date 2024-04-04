using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CoinsCollectable : Collectable
{
    public override void Interact()
    {
        base.Interact();
        Debug.LogWarning($"stuff is: {this.name}");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
