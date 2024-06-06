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
    //character classes

    [SerializeField]NetworkObject TestObject;
    private Dictionary<PlayerRef, NetworkObject> spawnCharacter = new Dictionary<PlayerRef, NetworkObject>();
    float playersJoined = 0f;

    //Session List
    SessionListUIHandler sessionListUIHandler;

    //Input
    CharacterInputHandler characterInputHandler;
    
    private void Awake()
    {
        sessionListUIHandler = FindObjectOfType<SessionListUIHandler>(true);
    }
    private void Start()
    {
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (characterInputHandler == null && Player.Local != null)
        {
            characterInputHandler = Player.Local.GetComponent<CharacterInputHandler>();
            //Debug.LogWarning($"getting Input Handler");

        }
        if (characterInputHandler != null)
        {
            input.Set(characterInputHandler.GetNetworkInput());
        }
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer) 
            return;
        if (SceneManager.GetActiveScene().name == "MenuScene") 
            return;

        Vector3 playerPos = new Vector3(0, 3f, playersJoined+2f);

        runner.Spawn(TestObject,playerPos, Quaternion.identity, player);

        playersJoined++;
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
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
        //Debug.LogError($"HOST LEFT THE GAME RRRAAAAAAHHHHH {shutdownReason}");

        StartCoroutine(HostLeft());
    }
    IEnumerator HostLeft()
    {
        SceneManager.LoadScene(0);

        yield return new WaitForSeconds(1f);
    }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }
}
