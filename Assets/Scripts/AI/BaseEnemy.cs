using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour
{
    public List<Player> players = new List<Player>();
    private Player target;
    private NavMeshAgent agent;
    private bool in_range = false;
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
            agent.destination = target.gameObject.transform.position;
            if (in_range)
            {
                combatController.Attack();
            }
            Vector3 targetLookAt = new Vector3(target.gameObject.transform.position.x, this.transform.position.y, target.gameObject.transform.position.z);
            transform.LookAt(targetLookAt);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            in_range = true;
            target = other.GetComponent<Player>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            in_range = false;
            target = null;
        }
    }
}
