using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Fusion;
[RequireComponent(
    typeof(NetworkMecanimAnimator)
    )]
public class BaseEnemy : NetworkBehaviour
{
    private Player[] targets = new Player[9];
    private Player target;
    private NavMeshAgent agent;
    private bool inRange = false;
    private CombatController combatController;
    private Animator anim;
    
    [SerializeField] private float speed;
    [SerializeField] private float acceleration;
    [SerializeField] private float stoppingDistance;

    [SerializeField] private float attackMaxTime;
    private float attackTimer;
    private bool canAttack = true;

    public override void Spawned()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.enabled = true;
        combatController = gameObject.GetComponent<CombatController>();
        attackTimer = attackMaxTime;
        anim = gameObject.GetComponent<Animator>();
        agent.speed = speed;
        agent.acceleration = acceleration;
        agent.stoppingDistance = stoppingDistance;
    }
    public override void FixedUpdateNetwork()
    {
        if (attackTimer > 0f && !canAttack)
        {
            attackTimer -= Time.deltaTime;
        } else if (attackTimer <= 0f && !canAttack)
        {
            attackTimer = 0f;
            canAttack = true;
        }
        if (target)
        {
            agent.destination = target.gameObject.transform.position;
            if (inRange && canAttack)
            {
                attackTimer = attackMaxTime;
                canAttack = false;
                anim.SetBool("Attacking", true);
            }
            else
            {
                Vector3 targetLookAt = new Vector3(target.gameObject.transform.position.x, this.transform.position.y, target.gameObject.transform.position.z);
                transform.LookAt(targetLookAt);
                anim.SetBool("Attacking", false);
                anim.SetBool("Moving", true);
            }
        }
        else
        {
            anim.SetBool("Moving", false);
            anim.SetBool("Attacking", false);
        }
    }

    public void InRangeSetter(bool set)
    {
        inRange = set;
    }

    public void TargetSetter(Collider[] set)
    {
        if (set[0] != null)
        {
            Player[] playerTargets = new Player[9];
            for (int i = 0; i < set.Length; i++)
            {
                if (set[i] != null)
                {
                    playerTargets[i] = set[i].GetComponent<Player>();
                }
            }
            targets = playerTargets;
            target = targets[0];
        } else target = null;
    }
}
