using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Player : NetworkBehaviour {

    [SerializeField] private float turnSpeed = 360f;
    [SerializeField] CharacterStatsScrObj Character;
    [SerializeField] private Transform camTarget;
    //[SerializeField]Characters currentCharacter;

    public static Vector3 m_MousePosition;
    private NetworkCharacterController characterController;
    private Animator anim;

    //the time the attack animation locks the player's rotation
    private float attackLength = 1.6f;
    private float attackTime   = 0f;

    //keep a reference to the skew to save a bit of calculation (you could also rotate the world and remove this)
    private readonly Quaternion skew = Quaternion.Euler(0, 45, 0);

    private bool isAttacking = false;
    //getter/setter for isAttacking which automatically sets the variables to start the timer and animation
    public bool IsAttacking {
		get { return isAttacking; }
		set {
            isAttacking = value;
            if(isAttacking) attackTime = attackLength;
            anim.SetBool("Attacking", isAttacking);
        }
	}

    public override void Spawned(){
        if (HasInputAuthority) CameraFollow.Singleton.SetTarget(camTarget);
    }

    private void Awake(){
        characterController = GetComponent<NetworkCharacterController>();
        characterController.maxSpeed = Character.MovementStats.MovementSpeed;
        anim = GetComponent<Animator>();
        //cam = FindObjectOfType<Camera>();
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
