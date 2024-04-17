using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
[RequireComponent(
    typeof(NetworkCharacterController),
    typeof(NetworkMecanimAnimator),
    typeof(Collector)
    )]
public class Player : NetworkBehaviour
{
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

    [Header("knockBack variables")]
    //Knockback and Push variables
    float knockBackCounter;
    [SerializeField] float knockBackTime = 5f;
    [SerializeField] float knockBackForce = 100f;

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
        if (HasInputAuthority)
        {
            localCamera = Instantiate(cam);
            localCamera.GetComponent<LocalCamera>().SetTarget(camTarget);
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
    public override void FixedUpdateNetwork()
    {
        //count down attack time and end the attack once it hits 0
        if (IsAttacking && attackTime > 0)
        {
            attackTime -= Time.deltaTime;
            if (attackTime <= 0) IsAttacking = false;
        }

        //exit if there is no input
        if (!GetInput(out NetworkInputData data)) return;

        //initiate attack with mouse click (also checks if the player is already attacking so the animation does not restart)
        if (data.buttons.IsSet(MyButtons.LeftClick) && !IsAttacking) IsAttacking = true;

        var dir = MousePosition.InWorldRayPosition - transform.position;
        dir.Normalize();
        //Vector3 forward = Vector3.zero;
        KnockBackHandler(data);
        if (IsAttacking)
        {
            FaceTo();
        }

        //movement blending variables
        WalkAnim();

        //Interaction using E
        m_Collector.SetInteractionBool(data.buttons.IsSet(MyButtons.InteractButton));

        if (data.buttons.IsSet(MyButtons.LeftClick))
        {
            //m_CombatController.Attack();
            // apply the impact force:
            if (knockBackCounter <= 0)
            {
                forward = ApplyForce(forward, dir);
            }
        }

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
        if (knockBackCounter <= 0)
        {
            if (data.direction != Vector3.zero)
            {
                forward = skew * data.direction;
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
        }
        //if there is a knockback then the player won't be able to move until the knockback is done
        else
        {
            knockBackCounter -= Runner.DeltaTime;

        }
    }

    private void FaceTo()
    {
        Vector3 direction = (MousePosition.InWorldRayPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Runner.DeltaTime * 90);
    }
    public Vector3 ApplyForce( Vector3 pMoveDirection, Vector3 pForceDirection)
    {
        knockBackCounter = knockBackTime;
        //Debug.LogWarning($"knockback!!!!!!!!!");
        return pForceDirection * knockBackForce;
    }
}
