using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Player : NetworkBehaviour
{
    [SerializeField] CharacterStatsScrObj TheMule;
    [SerializeField] CharacterStatsScrObj TheBoomstick;
    [SerializeField] CharacterStatsScrObj TheSiren;

    [SerializeField]Characters currentCharacter;

    private NetworkCharacterController characterController;

    [SerializeField] private Transform camTarget;

    float turnSpeed = 360f;
    float speed = 300f;

    [SerializeField] Animator anim;
    public override void Spawned()
    {
        if (HasInputAuthority)
        {
            CameraFollow.Singleton.SetTarget(camTarget);
        }
    }
    private void Awake()
    {
        characterController = GetComponent<NetworkCharacterController>();
        anim = GetComponent<Animator>();
    }
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            anim.SetBool("Moving", true);
            var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));

            var skewedInput = matrix.MultiplyPoint3x4(data.direction);

            if (data.direction != Vector3.zero)
            {
                var relative = (transform.position + data.direction) - transform.position;
                var rot = Quaternion.LookRotation(relative, Vector3.up);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Runner.DeltaTime);
            }
            //data.direction.Normalize();
            characterController.Move(Runner.DeltaTime * speed * skewedInput);

            if (data.direction == Vector3.zero)
                anim.SetBool("Moving", false);
        }



    }
}
