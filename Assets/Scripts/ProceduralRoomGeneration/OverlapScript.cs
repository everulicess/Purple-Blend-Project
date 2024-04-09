using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlapScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rooms"))
        {
            GetComponentInParent<RoomTemplates>().overlap = true;
        }
    }
}
