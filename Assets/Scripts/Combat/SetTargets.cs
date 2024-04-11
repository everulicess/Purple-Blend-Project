using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTargets : MonoBehaviour
{
    CombatController m_CombatController;
    string thisObjectTag;
    List<Health> targets = new();
    private void Start()
    {
        m_CombatController = GetComponentInParent<CombatController>();
        thisObjectTag = m_CombatController.tag;
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"COLLIDING WITH {other.name}");
        if (m_CombatController == null) return;
        if (other.CompareTag(thisObjectTag)) return;
        //Gets the Health script
        other.TryGetComponent(out Health health);
        // Adds object to potential list of targets.
        if (health != null)
        {
            targets.Add(health);
        }
        m_CombatController.UpdateTargetList(targets, true);
    }
    private void OnTriggerExit(Collider other)
    {
        if (m_CombatController == null) return;
        targets.Remove(other.gameObject.GetComponent<Health>());
    }
}
