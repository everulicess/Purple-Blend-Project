using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Player : NetworkBehaviour
{
    private NetworkCharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<NetworkCharacterController>();

    }
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out InputHandler data))
        {
            data.direction.Normalize();
            characterController.Move(10 * data.direction * Runner.DeltaTime);

        }
    }
}
