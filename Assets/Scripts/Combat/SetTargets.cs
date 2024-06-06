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

    private void OnTriggerStay(Collider other)
    {
        if (m_CombatController == null) return;
        //Gets the Health script
        other.TryGetComponent(out Health health);
        // Adds object to potential list of targets.
        if (health != null)
        {
            if (!targets.Contains(health))
            {
                targets.Add(health);
            }
        }
        m_CombatController.UpdateTargetList(targets);
    }
    public void ClearTargets()
    {
        if (m_CombatController == null) return;
        targets.Clear();
    }
}
