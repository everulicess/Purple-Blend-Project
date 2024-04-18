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

    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        combatController = gameObject.GetComponent<CombatController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (target)
        {
            if (inRange)
            {
                combatController.Attack();
                anim.SetBool("Attacking", true);
            }
            else
            {
                agent.destination = target.gameObject.transform.position;
                Vector3 targetLookAt = new Vector3(target.gameObject.transform.position.x, this.transform.position.y, target.gameObject.transform.position.z);
                transform.LookAt(targetLookAt);
                anim.SetBool("Attacking", false);
                anim.SetBool("Moving", true);
            }
        }
        else
        {
            anim.SetBool("Moving", false);
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
