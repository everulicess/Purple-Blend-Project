using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamageTest : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent<IDamageable>(out IDamageable damageable);
        if (damageable!= null)
        {
            damageable.OnTakeDamage(0.25f);
        }
    }
}
