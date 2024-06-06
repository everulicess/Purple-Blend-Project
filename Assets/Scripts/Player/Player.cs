using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using TMPro;
//enum PlayerState
//{
//    Walking_State,
//    Attacking_State,
//    Dodging_State,
//    Dead_State
//}
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
    [SerializeField] private GameObject escapeMenu;
    //[SerializeField]Characters currentCharacter;

    public static Vector3 m_MousePosition;
    private NetworkCharacterController m_CharacterController;
    private Animator anim;
    private CombatController m_CombatController;
    InGameMenu m_InGameMenu;
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

    [Header("Dodge variables")]
    bool isDodging = false;
    bool canDodge = true;
    float dodgeTime = 0.2f;
    float dodgeCooldown = 10f;
    float currentDodgeCooldown;
    [SerializeField] TextMeshProUGUI counterText;
    PlayerRef myUserID;
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;
            localCamera = Instantiate(cam);
            localCamera.GetComponent<LocalCamera>().SetTarget(camTarget);
            PlayerCamera = localCamera.GetComponentInChildren<Camera>();
            //Debug.LogError($"RPC with character is {PlayerPrefs.GetString("Character")}");
        }
        m_InGameMenu = GetComponentInChildren<InGameMenu>();
        m_CharacterController = GetComponent<NetworkCharacterController>();
        m_CharacterController.maxSpeed = Character.MovementStats.MovementSpeed;
        anim = GetComponent<Animator>();
        m_Collector = GetComponent<Collector>();
        m_Health = GetComponent<Health>();
        m_CombatController = GetComponent<CombatController>();
    }
    public void SetUserID(PlayerRef ID)
    {
        myUserID = ID;
        Debug.LogError($"MY player ID is {ID}");
    }
    public override void FixedUpdateNetwork()
    {
        HandleDeath();
        Falling();
        if (m_Health.isDead)
            return;
        //count down attack time and end the attack once it hits 0
        if (IsAttacking && attackTime > 0)
        {
            attackTime -= Time.deltaTime;
            if (attackTime <= 0 && !isDodging) IsAttacking = false;
        }

        //exit if there is no input
        if (!GetInput(out NetworkInputData data) && !Runner.IsForward) return;

        KnockBackHandler(data);
        if (m_CombatController.ranged) m_CombatController.attackArea.transform.position = data.pointToLookAt;
        //initiate attack with mouse click (also checks if the player is already attacking so the animation does not restart)
        if (data.buttons.IsSet(MyButtons.AttackButton) && !IsAttacking && !isDodging) 
        {
            IsAttacking = true;
            FaceTo(data);
        }
        if (data.buttons.IsSet(MyButtons.SpecialButton) && !IsAttacking && !isDodging)
        {
            IsAttacking = true;
            m_CombatController.special = true;
            FaceTo(data);
        }

        //Open the menu when menu button pressed
        m_InGameMenu.SetMenuInteraction(data.buttons.IsSet(MyButtons.MenuButton));

        //movement blending variables
        WalkAnim();

        if (data.buttons.IsSet(MyButtons.DodgeButton) && !isDodging)
            Dodge(data);
        ResetDodge();

        //Interaction using E
        m_Collector.SetInteractionBool(data.buttons.IsSet(MyButtons.InteractButton));

        isCarrying = m_Collector.GetCarryingBool();
        anim.SetBool("isCarrying", isCarrying);

        //move the character
        m_CharacterController.Move(forward);
        anim.SetBool("Moving", m_CharacterController.Velocity != Vector3.zero);

        // Test button to damage player character
        if(data.buttons.IsSet(MyButtons.TestingButtonQ))
            m_Health.OnTakeDamage(10);
    }

    private void Dodge(NetworkInputData data)
    {
        
        if (!canDodge)
            return;
        anim.SetTrigger("Dash");
        StartCoroutine(Dodging(data));
    }
    IEnumerator Dodging(NetworkInputData data)
    {
        //forward = skew * data.movementInput*500f;
        m_CharacterController.acceleration *= 2f;
        m_CharacterController.maxSpeed *= 2f;
        IsAttacking = false;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(forward, Vector3.up), turnSpeed * 90f);
        isDodging = true;
        canDodge = false;
        yield return new WaitForSeconds(0.5f);
        m_CharacterController.acceleration = 0f;
        m_CharacterController.maxSpeed = 0f;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(forward, Vector3.up), turnSpeed * 90f);
        isDodging = true;
        IsAttacking = false;
        canDodge = false;
        yield return new WaitForSeconds(dodgeTime);

        //starts countdown
        isDodging = false;
        canDodge = false;

        m_CharacterController.acceleration = Character.MovementStats.MovementSpeed;
        m_CharacterController.maxSpeed = Character.MovementStats.MovementSpeed;
        currentDodgeCooldown = dodgeCooldown;
    }
    private void ResetDodge()
    {
        if (currentDodgeCooldown > 0)
        {
            currentDodgeCooldown -= Runner.DeltaTime;
            counterText.text = currentDodgeCooldown.ToString("0.0");
        }
        else
        {
            canDodge = true;
        }
    }

    private void HandleDeath()
    {
        if (m_Health.isDead)
            m_CharacterController.Teleport(new Vector3(0, 1.5f, 0));
    }
    private void Falling()
    {
        if (transform.position.y <= -7f)
            m_Health.OnTakeDamage(100);
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

        if (data.movementInput != Vector3.zero && !isDodging) 
        {
            forward = skew * data.movementInput;
            //rotate the character unless it is currently attacking. The NetworkCharacterController's rotation speed should be 0 so it doesn't interfere with this.
            if (!IsAttacking)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(forward, Vector3.up), turnSpeed * Runner.DeltaTime);
            }
            forward.Normalize();
        }
        else if (!isDodging)
        {
            forward = Vector3.zero;
        }

    }
    private void FaceTo(NetworkInputData _data)
    {
        if (Player.Local == null) 
            return;

        Vector3 relativePos = new(_data.pointToLookAt.x - transform.position.x, 0f, _data.pointToLookAt.z - transform.position.z);
        Quaternion lookRotation = Quaternion.LookRotation(relativePos, Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Runner.DeltaTime * 90);
    }
    public void OnRespawn()
    {
        m_CharacterController.Teleport(new Vector3(0f, 1f, 0f));
    }

    //Handles what happens when the player leaves
    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
    }

    //Open player menu when pressing escape
    public void OpenMenu()
    {
        escapeMenu.gameObject.SetActive(!escapeMenu.activeInHierarchy);
    }
}
