using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;

public class BoxPlaceholderScript : MonoBehaviour
{
    public int health;

    public void Damaged(int damage)
    {
        Debug.Log(health);
        health -= damage;
        if(health <= 0)
        {
            GameObject.Find("Capsule").GetComponent<Combat>().targets.Remove(this);
            Destroy(this.gameObject);
        }
    }
}
