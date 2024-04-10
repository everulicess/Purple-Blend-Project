using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;

public class BoxPlaceholderScript : MonoBehaviour
{
    public float health;
    public Material damageMaterial;
    public Material normalMaterial;

    public void Damaged(float damage)
    {
        Debug.Log(health);
        health -= damage;
        gameObject.GetComponent<Renderer>().material = damageMaterial;
        if(health <= 0)
        {
            //GameObject.Find("Player").GetComponent<CombatController>().targets.Remove(this);
            Destroy(this.gameObject);
        }
        Invoke("ChangeMaterial", 0.1f);
        
    }

    public void ApplyKnockback(Vector3 knockbackVector)
    {
        gameObject.GetComponent<Rigidbody>().AddForce(knockbackVector, ForceMode.Impulse);
    }

    private void ChangeMaterial()
    {
        gameObject.GetComponent<Renderer>().material = normalMaterial;
    }
}
