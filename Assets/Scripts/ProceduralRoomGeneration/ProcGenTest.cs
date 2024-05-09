using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ProcGenTest : MonoBehaviour
{
    [SerializeField] private Vector2 gridSize;
    [SerializeField] private int dungeonSize;
    [SerializeField] private Vector2 startPos;
    [SerializeField] private int offset;
    [SerializeField] private List<GameObject> rooms = new List<GameObject>();

    private List<List<bool[]>> matrix = new List<List<bool[]>>();
    private List<Vector2> generatedRooms = new List<Vector2>();

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        //make grid
        for (int x = 0; x < gridSize.x; x++)
        {
            List<bool[]> column = new List<bool[]>();
            for (int y = 0; y < gridSize.y; y++)
            {
                bool[] doors = { false, false, false, false };
                column.Add(doors);
            }
            matrix.Add(column);
        }

        int roomCount = 0;
        Vector2 startRoomIndex = startPos;
        generatedRooms.Add(startRoomIndex);
        while (roomCount < dungeonSize)
        {
            //choose a random room from all the generated ones
            Vector2 randomRoomIndex = generatedRooms[Random.Range(0, generatedRooms.Count - 1)];
            //convert coordinates (floats) into ints
            int randomX = (int)randomRoomIndex.x;
            int randomY = (int)randomRoomIndex.y;
            //go to next loop iteration if all doors on chosen room are already connected
            if (matrix[randomX][randomY][0] == true && matrix[randomX][randomY][1] == true && matrix[randomX][randomY][2] == true && matrix[randomX][randomY][3] == true) continue;
            //pick a random spot next to the chosen room
            Vector2 room = new Vector2(randomX + Random.Range(-1, 2), randomY + Random.Range(-1, 2));
            //go to next loop iteration if spot is the same one as the room
            if (room == randomRoomIndex) continue;
            //float to int conversion
            int roomX = (int)room.x;
            int roomY = (int)room.y;
            if (room == new Vector2(randomX + 1, randomY + 1) || room == new Vector2(randomX + 1, randomY - 1) || room == new Vector2(randomX - 1, randomY + 1) || room == new Vector2(randomX - 1, randomY - 1)) continue;
            //if chosen spot is not equal to 0 (meaning it is already taken by a room), go to next loop iteration
            if (matrix[roomX][roomY].Contains(true)) continue;

            int openedDoor = 0;

            if (roomX == randomX + 1)
            {
                matrix[roomX][roomY][2] = true;
                openedDoor = 0;
            }
            if (roomX == randomX - 1)
            {
                matrix[roomX][roomY][0] = true;
                openedDoor = 2;
            }
            if (roomY == randomY + 1)
            {
                matrix[roomX][roomY][1] = true;
                openedDoor = 3;
            }
            if (roomY == randomY - 1)
            {
                matrix[roomX][roomY][3] = true;
                openedDoor = 1;
            }

            roomCount++;
            generatedRooms.Add(room);
            matrix[randomX][randomY][openedDoor] = true;
        }

        PlaceRooms();
    }

    private void PlaceRooms()
    {
        for (int i = 0; i < generatedRooms.Count - 1; i++)
        {
            GameObject room = Instantiate(rooms[0]);
            room.transform.position = new Vector3((generatedRooms[i].x * offset) - (generatedRooms[0].x * offset), 0f, (generatedRooms[i].y * offset)- (generatedRooms[0].y * offset));
            int roomX = (int)generatedRooms[i].x;
            int roomY = (int)generatedRooms[i].y;
            for (int door = 0; door < 4; door++)
            {
                if (matrix[roomX][roomY][door])
                {
                    Destroy(room.transform.GetChild(2).gameObject.transform.GetChild(door).gameObject);
                }
            }
        }
    }
}
