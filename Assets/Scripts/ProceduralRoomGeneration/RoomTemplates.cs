using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour
{
    public bool[] doorAvailability = { true, true, true, true };
    public Vector3 roomSize;
    public bool overlap;

    private BoxCollider overlapArea;

    private void Start()
    {
        overlapArea = transform.GetChild(1).GetComponent<BoxCollider>();
    }

    public void InitializeRoomData()
    {
        for (int i = 0; i < doorAvailability.Length; i++)
        {
            doorAvailability[i] = true;
        }
    }

    public void PlaceDoors()
    {
        for (int i = 0; i < doorAvailability.Length; i++)
        {
            if (!doorAvailability[i])
            {
                Destroy(transform.GetChild(2).GetChild(i).gameObject);
            }
        }
    }
}
