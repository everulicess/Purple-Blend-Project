using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.AI.Navigation;
using UnityEngine;
using TMPro;

public class MapManager : NetworkBehaviour
{
    [SerializeField] private Vector2 gridSize;
    [SerializeField] private int dungeonSize;
    [SerializeField] private Vector2 startPos;
    [SerializeField] private int offset;
    [SerializeField] private List<NetworkObject> net_Rooms = new();
    [SerializeField] private NavMeshSurface navMeshSurface;
    [SerializeField] private Camera mainCam;

    private float endCountdownTime;
    [SerializeField] private float maxEndCountdownTime;

    private List<List<bool[]>> matrix = new List<List<bool[]>>();
    private List<Vector2> generatedRooms = new List<Vector2>();

    private List<NetworkObject> rooms = new List<NetworkObject>();

    [Networked] public float totalGold { get; set; }
    public bool canGameEnd;
    public bool canStartCountdown;
    private bool gameEnded = false;

    [Networked] bool areRoomsSpawned { get; set; }
    public override void Spawned()
    {
        if (areRoomsSpawned) return;
        GenerateGrid();
    }

    public override void FixedUpdateNetwork()
    {
        if (canStartCountdown) EndGameCountdown();
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
            int roomIndex = 0;
            if (i != 0)
            {
                roomIndex = Random.Range(1, net_Rooms.Count);
            }
            NetworkObject net_room = Runner.Spawn(net_Rooms[roomIndex]);
            net_room.transform.position = new Vector3((generatedRooms[i].x * offset) - (generatedRooms[0].x * offset), 0f, (generatedRooms[i].y * offset) - (generatedRooms[0].y * offset));
            int net_roomX = (int)generatedRooms[i].x;
            int net_roomY = (int)generatedRooms[i].y;
            for (int door = 0; door < 4; door++)
            {
                if (matrix[net_roomX][net_roomY][door])
                {
                    Runner.Despawn(net_room.transform.GetChild(3).gameObject.transform.GetChild(door).GetComponent<NetworkObject>());
                }
            }
            AddGoldCount(net_room);
            rooms.Add(net_room);
        }
        areRoomsSpawned = true;
        Invoke(nameof(StartEnemySpawning), 1f);
    }

    private void StartEnemySpawning()
    {
        Invoke(nameof(NaveMeshBuild), 0.5f);
        for (int i = 0; i < generatedRooms.Count - 1; i++)
        {
            if (rooms[i].transform.GetChild(4).childCount > 0)
            {
                for (int spawner = 0; spawner < rooms[i].transform.GetChild(4).childCount; spawner++)
                {
                    if (rooms[i].transform.GetChild(4).childCount != 0)
                    {
                        rooms[i].transform.GetChild(4).transform.GetChild(spawner).TryGetComponent(out BaseSpawner baseSpawner);
                        if (baseSpawner != null)
                            baseSpawner.SpawnEnemy();
                    }
                }
            }
        }
    }

    private void NaveMeshBuild()
    {
        navMeshSurface.BuildNavMesh();
        GameObject.Find("Deposit").GetComponent<Deposit>().UpdateTotalMapGold();
    }

    private void AddGoldCount(NetworkObject curRoom)
    {
        GameObject collectables = curRoom.transform.GetChild(5).gameObject;
        for (int g = 0; g < collectables.transform.childCount; g++)
        {
            if (collectables.transform.GetChild(g).GetComponent<Collectable>())
            {
                totalGold += collectables.transform.GetChild(g).GetComponent<Collectable>().goldValue;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (canGameEnd)
        {
            other.GetComponent<Player>().canEndGame = true;
        }
    }
    private void EndGameCountdown()
    {
        if (endCountdownTime > 0 && gameEnded == false)
        {
            endCountdownTime -= Time.deltaTime;
        } else
        {
            gameEnded = true;
            endCountdownTime = 0;
            Player[] players = FindObjectsOfType<Player>();
            for (int i = 0; i < players.Length; i++)
            {
                players[i].ChangeCamera();
            }
            RPC_ActivateCamera(true);
        }
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_ActivateCamera(bool activateCanvas, RpcInfo info = default)
    {
        Debug.Log("pass1");
        RPC_ActivateCamera2( activateCanvas, info.Source);
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.InputAuthority, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_ActivateCamera2( bool activateCanvas, PlayerRef messageSource)
    {
        Debug.Log("pass2");
        if (messageSource == Runner.LocalPlayer)
        {
            Debug.Log("pass3");
            mainCam.gameObject.SetActive(activateCanvas);
        } else
        {
            mainCam.gameObject.SetActive(activateCanvas);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void QuitSession()
    {
        Runner.Shutdown();
    }
}
