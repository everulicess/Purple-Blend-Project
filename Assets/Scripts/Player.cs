using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Player : NetworkBehaviour
{
    private NetworkCharacterController characterController;

    [SerializeField] private Transform camTarget;

    public override void Spawned()
    {
        if (/*HasStateAuthority*/ HasInputAuthority)
        {
            
            CameraFollow.Singleton.SetTarget(camTarget);
        }
    }
    private void Awake()
    {
        characterController = GetComponent<NetworkCharacterController>();

    }
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            characterController.Move(10 * data.direction * Runner.DeltaTime);
        }
    }
}
