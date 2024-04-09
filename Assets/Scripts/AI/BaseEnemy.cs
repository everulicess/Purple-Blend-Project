using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour
{
    public List<Player> players = new List<Player>();
    private Player curTarget;
    private NavMeshAgent agent;
    private bool in_range = false;
    private CombatController combatController;

    // Start is called before the first frame update
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        combatController = gameObject.GetComponent<CombatController>();
        curTarget = players[0];
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = curTarget.gameObject.transform.position;
        if (in_range)
        {
            combatController.Attack();
        }
        Vector3 targetLookAt = new Vector3(curTarget.gameObject.transform.position.x, this.transform.position.y, curTarget.gameObject.transform.position.z);
        transform.LookAt(targetLookAt);
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
