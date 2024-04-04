using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class Collectable : MonoBehaviour
{
    public virtual void Interact()
    {
        Debug.LogWarning("collecting stuff");
    }
}
