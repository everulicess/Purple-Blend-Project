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
    private bool attacking = false;
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
            //Daan Animation Test
            if (Input.GetKey(KeyCode.Mouse0))
            {
                anim.SetBool("Attacking", true);
                Debug.Log("Attacking");
                attacking = true;
            }
            else
            {
                anim.SetBool("Attacking", false);
                attacking = false;
            }

            var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
            //data.direction.Normalize();

            var skewedInput = matrix.MultiplyPoint3x4(data.direction);

            if (data.direction != Vector3.zero)
            {
                var relative = (transform.position + data.direction) - transform.position;
                var rot = Quaternion.LookRotation(relative, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Runner.DeltaTime);
            }

            //Daan Testing animation calculation
            Vector3 fwdDirection;
            fwdDirection = transform.InverseTransformDirection(Vector3.forward);
            Debug.Log(fwdDirection);
            Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            Debug.Log(moveDirection);

            float lerpMinimum = -1f;  
            float lerpMaximum = 1f;
            Vector3 moveX = new Vector3(Mathf.Lerp(lerpMinimum, lerpMaximum, fwdDirection.x + moveDirection.x), 0, 0);
            Vector3 moveZ = new Vector3(0, 0, Mathf.Lerp(lerpMaximum, lerpMaximum, fwdDirection.z + moveDirection.z));
            Debug.Log($"Move x = {moveX}");
            Debug.Log($"Move Z = {moveZ}");
           
            float AnimationFloatX = moveX.x;
            float AnimationFloatZ = moveZ.z;
            anim.SetFloat("MoveX", AnimationFloatX);
            anim.SetFloat("MoveY", AnimationFloatZ);
            
            /*
            float AnimationFloatX = (fwdDirection.x + moveDirection.x)/2;
            float Animatianim.SetFloat("MoveX", AnimationFloatX);onFloatY = (fwdDirection.y + moveDirection.y)/2;
            anim.SetFloat("MoveX", AnimationFloatX);
            */
            

            skewedInput.Normalize();
            characterController.Move(Runner.DeltaTime * speed * skewedInput);
            //Debug.LogWarning($"skewed input is {skewedInput}");
            if (data.direction == Vector3.zero)
                anim.SetBool("Moving", false);

        }
    }
}
