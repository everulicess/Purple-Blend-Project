using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Fusion;
using UnityEngine.VFX;

public class CombatController : NetworkBehaviour
{
    public bool ranged;
    public bool special;

    public float comboMaxTime;
    public List<AttackTypesScrObj> attackTypes = new();
    public AttackTypesScrObj specialType;
    public GameObject hitbox;
    private SetTargets setTargets;

    private float damage;
    private float knockback;
    private float playerPush;
    private bool isAttacking;

    private int comboCounter = 0;
    private float comboTimeRemaining;
    private bool comboTimerIsRunning = false;
    private AttackTypesScrObj curAttack;

    private Animator animator;
    public GameObject attackArea;

    public Vector3 point;
    private float lookRotationSpeed = 8f;
    protected List<Health> targets = new();

    protected string thisObjectTag;

    [SerializeField] private CharacterEffectsScrObj effects;
    private AudioSource _audioSource;

    private AudioSource audioSource
    {
        get
        {
            if (_audioSource == null)
                _audioSource = GetComponent<AudioSource>();
            return _audioSource;
        }
    }
    // Start is called before the first frame update
    public override void Spawned()
    {
        // Assign values from AttackType Scriptable Object to the script and the attack area.
        thisObjectTag = this.tag;
        curAttack = attackTypes[0];
        damage = curAttack.damage;
        knockback = curAttack.knockback;
        playerPush = curAttack.playerPush;
        gameObject.transform.Find("AttackArea").GetComponent<MeshCollider>().sharedMesh = curAttack.colliderShape;
        gameObject.transform.Find("AttackArea").GetComponent<MeshFilter>().mesh = curAttack.colliderShape;
        comboTimeRemaining = comboMaxTime;
        setTargets = hitbox.AddComponent<SetTargets>();
        attackArea = gameObject.transform.Find("AttackArea").gameObject;
        animator = GetComponent<Animator>();

    }

    public override void FixedUpdateNetwork()
    {
        // Combo timer. When it reaches 0, the combo counter resets.
        if (comboTimerIsRunning)
        {
            if (comboCounter > 0 && comboTimeRemaining > 0)
            {
                comboTimeRemaining -= Time.deltaTime;
            }
            else
            {
                Debug.Log("combot reset");
                comboCounter = 0;
                animator.SetInteger("Combo", comboCounter);
                comboTimeRemaining = comboMaxTime;
                comboTimerIsRunning = false;
            }
        }
    }

    public void Attack()
    {
        if (!isAttacking)
        {
            // Switches isAttacking to true so that the player cannot spam attacks and invokes functions with a small delay.
            isAttacking = true;
            TryAttacking();
            Invoke(nameof(DisableIsAttacking), 0.3f);
        }
    }

    public void UpdateTargetList(List<Health> list) 
    {
        targets.Clear();
            foreach (Health item in list)
            {
                if (item == null) return;
                targets.Add(item);
                Debug.Log($"{item.name} has been added to the list");
            }
    }

    private void OnTriggerExit(Collider other)
    {
        // Removes object from potential list of targets.
        targets.Remove(other.gameObject.GetComponent<Health>());
    }

    private void TryAttacking()
    {
        SetAttackData();
        // Enables attack area's MeshRenderer to show the attack happening.
        hitbox.gameObject.GetComponent<MeshRenderer>().enabled = true;
        DamageTargets();
        IncreaseComboCounter();
        targets.Clear();
        switch (comboCounter)
        {
            case 0:
                audioSource.PlayOneShot(effects.Attack_1_Sound);

                ; break;
            case 1:
                audioSource.PlayOneShot(effects.Attack_2_Sound);

                ; break;
            case 2:
                audioSource.PlayOneShot(effects.Attack_3_Sound);
                ; break;
            default:
                audioSource.PlayOneShot(effects.Attack_1_Sound);
                break;
        }
    }

    // Checks through the list of objects within the targets list to damage them all.
    private void DamageTargets()
    {
        bool isHealing = false;
        foreach (Health target in targets)
        {
            float damageValue = damage;
            if (target != null)
            {
                isHealing = damageValue < 0;
                Vector3 target_tp = target.transform.position;
                Vector3 knockbackVector = (target_tp - gameObject.transform.position).normalized;
                target.TryGetComponent(out IDamageable damageable);
                if (damageable == null) return;
                if (target.CompareTag(tag))
                {
                    damageValue = isHealing ? damage : 0;
                } else damageValue = isHealing ? 0 : damage;
                
                damageable.OnTakeDamage(damageValue);
                //Debug.Log($"Dealing {damage} damage to {target.name}");
            }
        }
    }

    // Sets the attack data to the correct attack within the combo.
    private void SetAttackData()
    {
        if (!special) curAttack = attackTypes[comboCounter];
        else curAttack = specialType;
        damage = curAttack.damage;
        knockback = curAttack.knockback;
        playerPush = curAttack.playerPush * 100;
        attackArea.GetComponent<MeshCollider>().sharedMesh = curAttack.colliderShape;
        attackArea.GetComponent<MeshFilter>().mesh = curAttack.colliderShape;
    }

    // Increases the combo counter or resets it if it is done.
    private void IncreaseComboCounter()
    {
        if (comboCounter < attackTypes.Count - 1)
        {
            comboCounter++;
            comboTimeRemaining = comboMaxTime;
            comboTimerIsRunning = true;
        }
        else
        {
            comboCounter = 0;
            comboTimeRemaining = 0;
            comboTimerIsRunning = false;
        }
        animator.SetInteger("Combo", comboCounter);
    }
    private void DisableIsAttacking()
    {
        // Allows player to attack again and disables the attack area's MeshRenderer.
        isAttacking = false;
        special = false;
        hitbox.gameObject.GetComponent<MeshRenderer>().enabled = false;
        setTargets.ClearTargets();
    }
}
