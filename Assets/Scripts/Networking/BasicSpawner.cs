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
    NetworkPrefabRef networkPrefabRef;
    [SerializeField] NetworkPrefabRef networkPrefabRef_TheMule;
    [SerializeField] NetworkPrefabRef networkPrefabRef_TheBoomstick;
    [SerializeField] NetworkPrefabRef networkPrefabRef_TheSiren;

    private Dictionary<PlayerRef, NetworkObject> spawnCharacter = new Dictionary<PlayerRef, NetworkObject>();
    float playersJoined = 0f;

    SessionListUIHandler sessionListUIHandler;
    //Booleans for reseting input
    bool PingButtonPressed = false;
    bool InteractButtonPressed = false;
    bool TestingButtonQPressed = false;
    bool LeftClickPressed = false;
    bool RightClickPressed = false;

    NetworkPrefabRef GetCharacterToSpawn(string characterToSpawn)
    {
        return characterToSpawn switch
        {
            nameof(Characters.TheMule) => networkPrefabRef_TheMule,
            nameof(Characters.TheBoomstick) => networkPrefabRef_TheBoomstick,
            nameof(Characters.TheSiren) => networkPrefabRef_TheSiren,
            _ => networkPrefabRef_TheBoomstick,
        };
    }
    private void Awake()
    {
        sessionListUIHandler = FindObjectOfType<SessionListUIHandler>(true);
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        data.buttons.Set(MyButtons.PingsButton, Input.GetKeyDown(KeyCode.V) || PingButtonPressed);
        data.buttons.Set(MyButtons.InteractButton, Input.GetKeyDown(KeyCode.E) || InteractButtonPressed);
        data.buttons.Set(MyButtons.TestingButtonQ, Input.GetKeyDown(KeyCode.Q) || TestingButtonQPressed);
        data.buttons.Set(MyButtons.LeftClick, Input.GetMouseButtonDown(0) || LeftClickPressed);
        data.buttons.Set(MyButtons.RightClick, Input.GetMouseButtonDown(1) || RightClickPressed);
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
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer) return;
        if (SceneManager.GetActiveScene().name == "MenuScene") return;
        networkPrefabRef = GetCharacterToSpawn(PlayerPrefs.GetString("Character"));
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
    public async void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.LogError($"On Host Migration");
        await runner.Shutdown(shutdownReason: ShutdownReason.HostMigration);

        FindObjectOfType<NetworkRunnerHandler>().StartHostMigration(hostMigrationToken);
        Debug.LogError($"Method started");
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

            foreach (SessionInfo sessionInfo in sessionList)
            {
                sessionListUIHandler.AddToList(sessionInfo);
            }
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
    
    
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
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
    
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }
}
