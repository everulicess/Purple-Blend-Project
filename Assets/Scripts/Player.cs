using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Player : NetworkBehaviour
{
    [SerializeField] CharacterStatsScrObj Character;

    //[SerializeField]Characters currentCharacter;

    private NetworkCharacterController characterController;

    [SerializeField] private Transform camTarget;

    float turnSpeed = 360f;
    [SerializeField]float speed = 3f;

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
        characterController.maxSpeed = Character.MovementStats.MovementSpeed;
        
        if (GetInput(out NetworkInputData data))
        {
            anim.SetBool("Moving", true);
            var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
            //data.direction.Normalize();

            var skewedInput = matrix.MultiplyPoint3x4(data.direction);

            if (data.direction != Vector3.zero)
            {
                var relative = (transform.position + data.direction) - transform.position;
                var rot = Quaternion.LookRotation(relative, Vector3.up);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Runner.DeltaTime);
            }
            skewedInput.Normalize();
            characterController.Move(Runner.DeltaTime * speed * skewedInput);
            //Debug.LogWarning($"skewed input is {skewedInput}");
            if (data.direction == Vector3.zero)
                anim.SetBool("Moving", false);
        }
    }
}
