using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.AI.Navigation;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    [SerializeField] private int roomCount;
    [SerializeField] private Vector2 roomSize = new Vector2(10, 10);
    [SerializeField] private float generateTime = 0.05f;
    [SerializeField] private List<GameObject> roomLayouts = new List<GameObject>();
    [SerializeField] private NavMeshSurface navMeshSurface;

    private List<GameObject> generatedRooms = new List<GameObject>();
    private int selectedDoor = 0;

    private void Start()
    {
        generatedRooms.Add(gameObject);
        StartCoroutine(GenerateMap());
    }

    IEnumerator GenerateMap()
    {
        int counter = 0;
        while (counter < roomCount-1)
        {

            RoomTemplates selectedRoom = generatedRooms[Random.Range(0, generatedRooms.Count-1)].GetComponent<RoomTemplates>();
            int randomDoor = Random.Range(0, selectedRoom.doorAvailability.Length);
            for (int door = 0; door < selectedRoom.doorAvailability.Length; door++)
            {
                if (door == randomDoor)
                {
                    if (selectedRoom.doorAvailability[door] == true)
                    {
                        selectedDoor = door;
                        break;
                    } else
                    {
                        goto end_of_loop;
                    }
                }
            }

            int randomRoomIndex = Random.Range(0, roomLayouts.Count-1);
            GameObject room = Instantiate(roomLayouts[randomRoomIndex]);
            room.GetComponent<RoomTemplates>().roomSize = new Vector3(roomSize.x, room.GetComponent<RoomTemplates>().roomSize.y, roomSize.y);
            room.GetComponent<RoomTemplates>().InitializeRoomData();

            Vector3 objPos = selectedRoom.transform.position;
            if (selectedDoor == 0)
            {
                room.transform.position = new Vector3(objPos.x + roomSize.x, objPos.y, objPos.z);
                room.GetComponent<RoomTemplates>().doorAvailability[2] = false;
            }
            else if (selectedDoor == 1)
            {
                room.transform.position = new Vector3(objPos.x, objPos.y, objPos.z - roomSize.y);
                room.GetComponent<RoomTemplates>().doorAvailability[3] = false;
            }
            else if (selectedDoor == 2)
            {
                room.transform.position = new Vector3(objPos.x - roomSize.x, objPos.y, objPos.z);
                room.GetComponent<RoomTemplates>().doorAvailability[0] = false;
            }
            else if (selectedDoor == 3)
            {
                room.transform.position = new Vector3(objPos.x, objPos.y, objPos.z + roomSize.y);
                room.GetComponent<RoomTemplates>().doorAvailability[1] = false;
            }

            yield return new WaitForSeconds(generateTime / 2);
            if (room.GetComponent<RoomTemplates>().overlap == true)
            {
                Destroy(room);
            } else
            {
                generatedRooms.Add(room);
                selectedRoom.doorAvailability[selectedDoor] = false;
                counter++;
            }
            yield return new WaitForSeconds(generateTime / 2);
            end_of_loop: {}
        }
        for (int i = 0; i < generatedRooms.Count; i++)
        {
            generatedRooms[i].GetComponent<RoomTemplates>().PlaceDoors();
        }
        yield return new WaitForSeconds(generateTime / 2);
        navMeshSurface.BuildNavMesh();
    }
}
