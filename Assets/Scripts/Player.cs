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

    [SerializeField] GameObject cam;
    GameObject localCamera;
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
        if (HasInputAuthority) /*CameraFollow.Singleton.SetTarget(camTarget);*/
            localCamera = Instantiate(cam);
        localCamera.GetComponent<LocalCamera>().SetTarget(camTarget);
    }

    private void Awake(){
        characterController = GetComponent<NetworkCharacterController>();
        characterController.maxSpeed = Character.MovementStats.MovementSpeed;
        anim = GetComponent<Animator>();
        //cam = FindObjectOfType<Camera>();
    }
    public override void FixedUpdateNetwork()
    {

        //count down attack time and end the attack once it hits 0
        if (IsAttacking && attackTime > 0)
        {
            attackTime -= Runner.DeltaTime;
            if (attackTime <= 0) IsAttacking = false;
        }

        //exit if there is no input
        if (!GetInput(out NetworkInputData data)) return;

        //initiate attack with mouse click (also checks if the player is already attacking so the animation does not restart)
        if (Input.GetKey(KeyCode.Mouse0) && !IsAttacking) IsAttacking = true;

        Vector3 forward = Vector3.zero;
        if (data.direction != Vector3.zero)
        {
            forward = skew * data.direction;
            //rotate the character unless it is currently attacking. The NetworkCharacterController's rotation speed should be 0 so it doesn't interfere with this.
            if (!IsAttacking) transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(forward, Vector3.up), turnSpeed * Runner.DeltaTime);
        }

        //movement blending variables
        if (characterController.Velocity == Vector3.zero)
        {
            anim.SetFloat("MoveX", 0);
            anim.SetFloat("MoveY", 0);
        }
        else
        {
            //get the angle between the forward and movement vectors and multiply it by 2pi/360 to get the correct sine/cosine
            float angle = Vector3.SignedAngle(transform.forward, characterController.Velocity, Vector3.up) * 0.0174532925199f;
            //calculate the ratio between current and maximum speed to make blend walk and idle depending on speed
            float ratio = characterController.Velocity.magnitude / characterController.maxSpeed;
            //sine and cosine times ratio gives the final x and y values
            anim.SetFloat("MoveX", Mathf.Sin(angle) * ratio);
            anim.SetFloat("MoveY", Mathf.Cos(angle) * ratio);
        }

        //move the character
        characterController.Move(forward);
        anim.SetBool("Moving", characterController.Velocity != Vector3.zero);
    }
}
