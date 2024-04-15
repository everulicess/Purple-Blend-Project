using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildColliderScript : MonoBehaviour
{
    private BaseEnemy enemy;

    private void Start()
    {
        enemy = GetComponentInParent<BaseEnemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (CompareTag("HitBox"))
            {
                enemy.InRangeSetter(true);
            } else if (CompareTag("DetectionArea"))
            {
                enemy.TargetSetter(true, other.GetComponent<Player>());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (CompareTag("HitBox"))
            {
                enemy.InRangeSetter(false);
            }
            else if (CompareTag("DetectionArea"))
            {
                enemy.TargetSetter(false, other.GetComponent<Player>());
            }
        }
    }
}
