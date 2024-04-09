using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Fusion;

public class CombatController : NetworkBehaviour
{
    public float comboMaxTime;
    public List<AttackTypesScrObj> attackTypes = new List<AttackTypesScrObj>();
    public GameObject hitbox;

    private float damage;
    private float knockback;
    private float playerPush;
    private bool isAttacking;

    private int comboCounter = 0;
    private float comboTimeRemaining;
    private bool comboTimerIsRunning = false;
    private AttackTypesScrObj curAttack;

    public Vector3 point;
    private float lookRotationSpeed = 8f;
    public List<BoxPlaceholderScript> targets = new List<BoxPlaceholderScript>();

    // Start is called before the first frame update
    void Start()
    {
        // Assign values from AttackType Scriptable Object to the script and the attack area.
        curAttack = attackTypes[0];
        damage = curAttack.damage;
        knockback = curAttack.knockback;
        playerPush = curAttack.playerPush;
        gameObject.transform.Find("AttackArea").GetComponent<MeshCollider>().sharedMesh = curAttack.colliderShape;
        gameObject.transform.Find("AttackArea").GetComponent<MeshFilter>().mesh = curAttack.colliderShape;
        comboTimeRemaining = comboMaxTime;
    }

    public override void FixedUpdateNetwork()
    {
        FaceTarget();
        // Combo timer. When it reaches 0, the combo counter resets.
        if (comboTimerIsRunning)
        {
            if (comboCounter > 0 && comboTimeRemaining > 0)
            {
                comboTimeRemaining -= Time.deltaTime;
            }
            else
            {
                Debug.Log("combo reset");
                comboCounter = 0;
                comboTimeRemaining = comboMaxTime;
                comboTimerIsRunning = false;
            }
        }
    }

    private void FaceTarget()
    {
        // Turns the player towards the clicked spot.
        if (point != null)
        {
            Vector3 direction = (point - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
        }
    }

    public void Attack()
    {
        if (!isAttacking)
        {
            // Switches isAttacking to true so that the player cannot spam attacks and invokes functions with a small delay.
            isAttacking = true;
            Invoke("TryAttacking", 0.3f);
            Invoke("DisableIsAttacking", 0.5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Adds object to potential list of targets.
        if (other.gameObject.GetComponent<BoxPlaceholderScript>() != null)
        {
            targets.Add(other.gameObject.GetComponent<BoxPlaceholderScript>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Removes object from potential list of targets.
        targets.Remove(other.gameObject.GetComponent<BoxPlaceholderScript>());
    }

    private void TryAttacking()
    {
        SetAttackData();
        // Enables attack area's MeshRenderer to show the attack happening.
        hitbox.gameObject.GetComponent<MeshRenderer>().enabled = true;
        DamageTargets();
        //PushPlayerToAttack();
        IncreaseComboCounter();
        targets.Clear();
    }

    // Checks through the list of objects within the targets list to damage them all.
    private void DamageTargets()
    {
        foreach (BoxPlaceholderScript target in targets.ToList())
        {
            if (target != null)
            {
                Vector3 target_tp = target.transform.position;
                Vector3 knockbackVector = (target_tp - gameObject.transform.position).normalized;
                target.GetComponent<BoxPlaceholderScript>().ApplyKnockback(knockbackVector*knockback);
                target.GetComponent<BoxPlaceholderScript>().Damaged(damage);
            }
        }
    }

    // Sets the attack data to the correct attack within the combo.
    private void SetAttackData()
    {
        curAttack = attackTypes[comboCounter];
        damage = curAttack.damage;
        knockback = curAttack.knockback;
        playerPush = curAttack.playerPush * 100;
        gameObject.transform.Find("AttackArea").GetComponent<MeshCollider>().sharedMesh = curAttack.colliderShape;
        gameObject.transform.Find("AttackArea").GetComponent<MeshFilter>().mesh = curAttack.colliderShape;
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
    }
    private void DisableIsAttacking()
    {
        // Allows player to attack again and disables the attack area's MeshRenderer.
        isAttacking = false;
        hitbox.gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    //private void PushPlayerToAttack()
    //{
    //    gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(transform.forward.x * playerPush, transform.forward.y, transform.forward.z * playerPush));
    //}
}
