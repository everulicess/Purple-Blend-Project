using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.AI.Navigation;
using UnityEngine;

public class RoomSpawner : NetworkBehaviour
{
    public static RoomSpawner Singleton
    {
        get => singleton;
        set
        {
            if (value == null)
                singleton = null;
            else if (singleton == null)
                singleton = value;
            else if (singleton != value)
            {
                Destroy(value);
                Debug.LogError($"Only one instance of {nameof(RoomSpawner)}!");
            }
        }
    }

    private static RoomSpawner singleton;

    [SerializeField] private int roomCount;
    [SerializeField] private Vector2 roomSize = new Vector2(10, 10);
    [SerializeField] private float generateTime = 0.05f;
    [SerializeField] private List<NetworkObject> roomLayouts = new();
    [SerializeField] private NavMeshSurface navMeshSurface;
    [Networked] bool isSpawningRoomsDone { get; set; }
    private List<NetworkObject> generatedRooms = new();
    private int selectedDoor = 0;

    public override void Spawned()
    {
        if (isSpawningRoomsDone) return;
        generatedRooms.Add(gameObject.GetComponent<NetworkObject>());
        StartCoroutine(GenerateMap());
    }
    public IEnumerator GenerateMap()
    {
        if (!isSpawningRoomsDone)
        {
            int counter = 0;
            while (counter < roomCount - 1)
            {

                RoomTemplates selectedRoom = generatedRooms[Random.Range(0, generatedRooms.Count - 1)].GetComponent<RoomTemplates>();
                int randomDoor = Random.Range(0, selectedRoom.doorAvailability.Length);
                for (int door = 0; door < selectedRoom.doorAvailability.Length; door++)
                {
                    if (door == randomDoor)
                    {
                        if (selectedRoom.doorAvailability[door] == true)
                        {
                            selectedDoor = door;
                            break;
                        }
                        else
                        {
                            goto end_of_loop;
                        }
                    }
                }

                int randomRoomIndex = Random.Range(0, roomLayouts.Count - 1);
                NetworkObject net_room = Runner.Spawn(roomLayouts[randomRoomIndex].gameObject.GetComponent<NetworkObject>());
                net_room.GetComponent<RoomTemplates>().roomSize = new Vector3(roomSize.x, net_room.GetComponent<RoomTemplates>().roomSize.y, roomSize.y);
                net_room.GetComponent<RoomTemplates>().InitializeRoomData();

                Vector3 objPos = selectedRoom.transform.position;
                if (selectedDoor == 0)
                {
                    net_room.transform.position = new Vector3(objPos.x + roomSize.x, objPos.y, objPos.z);
                    net_room.GetComponent<RoomTemplates>().doorAvailability[2] = false;
                }
                else if (selectedDoor == 1)
                {
                    net_room.transform.position = new Vector3(objPos.x, objPos.y, objPos.z - roomSize.y);
                    net_room.GetComponent<RoomTemplates>().doorAvailability[3] = false;
                }
                else if (selectedDoor == 2)
                {
                    net_room.transform.position = new Vector3(objPos.x - roomSize.x, objPos.y, objPos.z);
                    net_room.GetComponent<RoomTemplates>().doorAvailability[0] = false;
                }
                else if (selectedDoor == 3)
                {
                    net_room.transform.position = new Vector3(objPos.x, objPos.y, objPos.z + roomSize.y);
                    net_room.GetComponent<RoomTemplates>().doorAvailability[1] = false;
                }

                yield return new WaitForSeconds(generateTime / 2);
                if (net_room.GetComponent<RoomTemplates>().overlap == true)
                {
                    Runner.Despawn(net_room);
                }
                else
                {
                    generatedRooms.Add(net_room);
                    selectedRoom.doorAvailability[selectedDoor] = false;
                    counter++;
                }
                yield return new WaitForSeconds(generateTime / 2);
            end_of_loop: { }
            }
            for (int i = 0; i < generatedRooms.Count; i++)
            {
                generatedRooms[i].GetComponent<RoomTemplates>().PlaceDoors();
            }
            yield return new WaitForSeconds(generateTime / 2);
            navMeshSurface.BuildNavMesh();
            isSpawningRoomsDone = true;
        }

    }
}
