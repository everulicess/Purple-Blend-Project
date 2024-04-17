using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using System;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{

    public NetworkRunner networkRunner;
    [SerializeField] NetworkPrefabRef networkPrefabRef;

    private Dictionary<PlayerRef, NetworkObject> spawnCharacter = new Dictionary<PlayerRef, NetworkObject>();
    //async void GameStart(GameMode mode)
    //{
    //    networkRunner = gameObject.AddComponent<NetworkRunner>();
    //    networkRunner.ProvideInput = true;

    //    var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
    //    var sceneInfo = new NetworkSceneInfo();

    //    if (scene.IsValid)
    //    {
    //        sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
    //    }

    //    await networkRunner.StartGame(new StartGameArgs()
    //    {
    //        GameMode = mode,
    //        SessionName = "Test Room",
    //        Scene = scene,
    //        SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
    //        //PlayerCount = 2
    //    });
    //}

    SessionListUIHandler sessionListUIHandler;

    private void Awake()
    {
        sessionListUIHandler = FindObjectOfType<SessionListUIHandler>(true);
    }

    private void OnGUI()
    {
        if (networkRunner == null)
        {
            //if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
            //{
            //    GameStart(GameMode.Host);
            //}
            //if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
            //{
            //    GameStart(GameMode.Client);
            //}
        }
    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public async void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.LogError($"On Host Migration");
        await runner.Shutdown(shutdownReason: ShutdownReason.HostMigration);

        FindObjectOfType<NetworkRunnerHandler>().StartHostMigration(hostMigrationToken);
        Debug.LogError($"Method started");
    }
    bool PingButtonPressed = false;
    bool InteractButtonPressed = false;
    bool TestingButtonQPressed = false;
    bool LeftClickPressed = false;
    bool RightClickPressed = false;
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        data.buttons.Set(MyButtons.PingsButton, Input.GetKeyDown(KeyCode.V)||PingButtonPressed);
        data.buttons.Set(MyButtons.InteractButton, Input.GetKeyDown(KeyCode.E)||InteractButtonPressed);
        data.buttons.Set(MyButtons.TestingButtonQ, Input.GetKeyDown(KeyCode.Q)||TestingButtonQPressed);
        data.buttons.Set(MyButtons.LeftClick, Input.GetMouseButtonDown(0)||LeftClickPressed);
        data.buttons.Set(MyButtons.RightClick, Input.GetMouseButtonDown(1)||RightClickPressed);
        data.direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        input.Set(data);

        //reset input
        InteractButtonPressed = false;
        TestingButtonQPressed = false;
        PingButtonPressed = false;
        LeftClickPressed = false;
        RightClickPressed = false;
    }
    private void Update()
    {
        TestingButtonQPressed = Input.GetKeyDown(KeyCode.Q);
        PingButtonPressed = Input.GetKeyDown(KeyCode.V);
        InteractButtonPressed = Input.GetKeyDown(KeyCode.E);
        LeftClickPressed = Input.GetMouseButtonDown(0);
        RightClickPressed = Input.GetMouseButtonDown(1);
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }
    float playersJoined = 0f;
    bool roomsInitialized = false;
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer) return;
        if (SceneManager.GetActiveScene().name == "MenuScene") return;
        //if (playersJoined<=1)
        //{
        //    StartCoroutine(FindObjectOfType<RoomSpawner>().GenerateMap());
        //    roomsInitialized = true;
        //}
        Vector3 playerPos = new(/*(player.RawEncoded % runner.Config.Simulation.PlayerCount) * */0 + playersJoined, 0f, 0f);

        NetworkObject networkObject = runner.Spawn(networkPrefabRef, playerPos, Quaternion.identity, player);

        playersJoined++;
        spawnCharacter.Add(player, networkObject);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (spawnCharacter.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            spawnCharacter.Remove(player);
        }
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        if (sessionListUIHandler == null) return;
        if (sessionList.Count == 0)
        {
            sessionListUIHandler.OnNoSessionFound();

        }
        else
        {
            sessionListUIHandler.ClearList();

            foreach(SessionInfo sessionInfo in sessionList)
            {
                sessionListUIHandler.AddToList(sessionInfo);
            }
        }
        
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

}
