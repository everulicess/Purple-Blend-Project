using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    private bool in_range = false;
    private CombatController combatController;

    // Start is called before the first frame update
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        combatController = gameObject.GetComponent<CombatController>();
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = player.position;
        if (in_range)
        {
            combatController.Attack();
        }
        transform.LookAt(player.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            in_range = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            in_range = false;
        }
    }
}
