using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
[RequireComponent(
    typeof(NetworkCharacterController),
    typeof(NetworkMecanimAnimator),
    typeof(Collector)
    )]
public class Player : NetworkBehaviour, IPlayerLeft
{
    public static Player Local { get; set; }

    public static Camera PlayerCamera { get; set; }

    [SerializeField] private float turnSpeed = 360f;
    [SerializeField] CharacterStatsScrObj Character;
    [SerializeField] private Transform camTarget;
    //[SerializeField]Characters currentCharacter;

    public static Vector3 m_MousePosition;
    private NetworkCharacterController m_CharacterController;
    private Animator anim;
    private CombatController m_CombatController;

    //the time the attack animation locks the player's rotation
    [SerializeField] private float attackLength = 1.6f;
    private float attackTime = 0f;

    //keep a reference to the skew to save a bit of calculation (you could also rotate the world and remove this)
    private readonly Quaternion skew = Quaternion.Euler(0, 45, 0);

    [SerializeField] GameObject cam;
    GameObject localCamera;
    private bool isAttacking = false;

    Collector m_Collector;
    Health m_Health;
    BasicSpawner basicSpawner;
    //Knockback and Push variables

    //movement direction
    Vector3 forward;

    //animation booleans
    bool isCarrying = false;
    //getter/setter for isAttacking which automatically sets the variables to start the timer and animation
    public bool IsAttacking
    {
        get { return isAttacking; }
        set
        {
            isAttacking = value;
            if (isAttacking) attackTime = attackLength;
            anim.SetBool("Attacking", isAttacking);
        }
    }
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;
            localCamera = Instantiate(cam);
            localCamera.GetComponent<LocalCamera>().SetTarget(camTarget);
            PlayerCamera = localCamera.GetComponentInChildren<Camera>();
        }
    }

    private void Awake()
    {
        m_CharacterController = GetComponent<NetworkCharacterController>();
        m_CharacterController.maxSpeed = Character.MovementStats.MovementSpeed;
        anim = GetComponent<Animator>();
        m_Collector = GetComponent<Collector>();
        m_Health = GetComponent<Health>();
        m_CombatController = GetComponent<CombatController>();
    }
    private void Start()
    {
        if (!HasInputAuthority) return;
            basicSpawner = FindObjectOfType<BasicSpawner>();
    }
    public override void FixedUpdateNetwork()
    {
        //count down attack time and end the attack once it hits 0
        if (IsAttacking && attackTime > 0)
        {
            attackTime -= Time.deltaTime;
            if (attackTime <= 0) IsAttacking = false;
        }

        //exit if there is no input
        if (!GetInput(out NetworkInputData data) && !Runner.IsForward) return;

        KnockBackHandler(data);

        //initiate attack with mouse click (also checks if the player is already attacking so the animation does not restart)
        if (data.buttons.IsSet(MyButtons.AttackButton) /*&& !IsAttacking*/)
        {
            IsAttacking = true;
            FaceTo(data);

        }
            
        //movement blending variables
        WalkAnim();

        //Interaction using E
        m_Collector.SetInteractionBool(data.buttons.IsSet(MyButtons.InteractButton));

        isCarrying = m_Collector.GetCarryingBool();
        anim.SetBool("isCarrying", isCarrying);

        //move the character
        m_CharacterController.Move(forward);
        anim.SetBool("Moving", m_CharacterController.Velocity != Vector3.zero);
    }
    
    private void WalkAnim()
    {
        if (m_CharacterController.Velocity == Vector3.zero)
        {
            anim.SetFloat("MoveX", 0);
            anim.SetFloat("MoveY", 0);
        }
        else
        {
            //get the angle between the forward and movement vectors and multiply it by 2pi/360 to get the correct sine/cosine
            float angle = Vector3.SignedAngle(transform.forward, m_CharacterController.Velocity, Vector3.up) * 0.0174532925199f;
            //calculate the ratio between current and maximum speed to make blend walk and idle depending on speed
            float ratio = m_CharacterController.Velocity.magnitude / m_CharacterController.maxSpeed;
            //sine and cosine times ratio gives the final x and y values
            anim.SetFloat("MoveX", Mathf.Sin(angle) * ratio);
            anim.SetFloat("MoveY", Mathf.Cos(angle) * ratio);
        }
    }

    private void KnockBackHandler(NetworkInputData data)
    {
        //check for the knockback, if there is no knockback then the player will be able to move
        
            //if (data.direction != Vector3.zero)
            //{
            //    forward = skew * data.direction;
            //    //rotate the character unless it is currently attacking. The NetworkCharacterController's rotation speed should be 0 so it doesn't interfere with this.
            //    if (!IsAttacking)
            //    {
            //        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(forward, Vector3.up), turnSpeed * Runner.DeltaTime);
            //    }
            //}
            //else
            //{
            //    forward = Vector3.zero;
            //}
        if (data.movementInput != Vector3.zero)
            {
                forward = skew * data.movementInput;
                //rotate the character unless it is currently attacking. The NetworkCharacterController's rotation speed should be 0 so it doesn't interfere with this.
                if (!IsAttacking)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(forward, Vector3.up), turnSpeed * Runner.DeltaTime);
                }
            }
            else
            {
                forward = Vector3.zero;
            }
        forward.Normalize();
    }
    private void FaceTo(NetworkInputData _data)
    {
        if (Player.Local == null) 
            return;

        Vector3 relativePos = new(_data.pointToLookAt.x - transform.position.x, 0f, _data.pointToLookAt.z - transform.position.z);
        Quaternion lookRotation = Quaternion.LookRotation(relativePos, Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Runner.DeltaTime * 90);
    }

    //Handles what happens when the player leaves
    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
    }
}
