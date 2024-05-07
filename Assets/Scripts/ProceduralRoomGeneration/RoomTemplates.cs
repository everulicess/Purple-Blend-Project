using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class RoomTemplates : NetworkBehaviour
{
    public bool[] doorAvailability = { true, true, true, true };
    //public bool[] net_DoorAvailability 
    public Vector3 roomSize;
    public bool overlap;

    private BoxCollider overlapArea;

    public override void Spawned()
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
                Runner.Despawn(transform.GetChild(2).GetChild(i).gameObject.GetComponent<NetworkObject>());
            }
        }
    }
}
