using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class testCharacter : NetworkBehaviour
{
    public override void Spawned()
    {
        Debug.LogError($"The Selected Character is {PlayerPrefs.GetString("Character")}");
    }
}
