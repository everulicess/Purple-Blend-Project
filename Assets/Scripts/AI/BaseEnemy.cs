using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour
{
    private List<Player> targets = new List<Player>();
    private Player target;
    private NavMeshAgent agent;
    private bool inRange = false;
    private CombatController combatController;

    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        combatController = gameObject.GetComponent<CombatController>();
    }

    void Update()
    {
        if (target)
        {
            if (inRange)
            {
                combatController.Attack();
            } else
            {
                agent.destination = target.gameObject.transform.position;
                Vector3 targetLookAt = new Vector3(target.gameObject.transform.position.x, this.transform.position.y, target.gameObject.transform.position.z);
                transform.LookAt(targetLookAt);
            }
        }
    }

    public void InRangeSetter(bool set)
    {
        inRange = set;
    }

    public void TargetSetter(Player set)
    {
        target = set;
    }
}
