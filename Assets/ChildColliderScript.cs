using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ChildColliderScript : MonoBehaviour
{
    private BaseEnemy enemy;

    private void Start()
    {
        enemy = GetComponentInParent<BaseEnemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent(out Player obj);
        if (obj == null) return;
        if (CompareTag("HitBox"))
        {
            enemy.InRangeSetter(true);
        }
        else if (CompareTag("DetectionArea"))
        {
            Debug.Log("detected");
            enemy.TargetSetter(true, other.GetComponent<Player>());
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
