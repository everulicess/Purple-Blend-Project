using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;

public class BoxPlaceholderScript : MonoBehaviour
{
    public float health;

    public void Damaged(float damage)
    {
        Debug.Log(health);
        health -= damage;
        if(health <= 0)
        {
            GameObject.Find("Capsule").GetComponent<Combat>().targets.Remove(this);
            Destroy(this.gameObject);
        }
    }

    public void ApplyKnockback(Vector3 knockbackVector)
    {
        gameObject.GetComponent<Rigidbody>().AddForce(knockbackVector, ForceMode.Impulse);
    }
}
