using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;
using System.Linq;

public class NetworkRunnerHandler : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;
    NetworkRunner networkRunner;
    private void Awake()
    {
        NetworkRunner networkRunnerInScene = FindObjectOfType<NetworkRunner>();

        if (networkRunnerInScene != null) networkRunner = networkRunnerInScene;
    }
    private void Start()
    {
        if (networkRunner == null)
        { 
            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.name = "Network runner";

            if (SceneManager.GetActiveScene().name!="MenuScene")
            {
                var clientTask = InitializeNetworkRunner(networkRunner, GameMode.AutoHostOrClient, "Test Session",NetAddress.Any(), SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex), 9);
            }
            Debug.Log($"Server Network Runner started");
        }
    }
    public void StartHostMigration(HostMigrationToken pHostMigrationToken)
    {
        Debug.LogError("migration is being called");
        networkRunner = Instantiate(networkRunnerPrefab);
        networkRunner.name = "Network runner - Migrated";

        var clientTask = InitializeNetworkRunnerHostMigration(networkRunner, pHostMigrationToken);
        Debug.LogError($"Host Migration started");
    }
    INetworkSceneManager GetSceneManager(NetworkRunner pRunner)
    {
        var sceneManager = pRunner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        if (sceneManager == null)
        {
            sceneManager = pRunner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }
        return sceneManager;
    }
    protected virtual Task InitializeNetworkRunner(NetworkRunner pRunner, GameMode pGameMode, string pSessionName, /*byte[] pConnectionToken,*/NetAddress pAddress, SceneRef pScene, int pPlayerCount)
    {
        var sceneManager = GetSceneManager(pRunner);

        pRunner.ProvideInput = true;

        return pRunner.StartGame(new StartGameArgs
        {
            GameMode = pGameMode,
            Address = pAddress,
            Scene = pScene,
            SessionName = pSessionName,
            CustomLobbyName = "Sunken Spoils Lobby",
            SceneManager = sceneManager,
            PlayerCount = pPlayerCount,
            //ConnectionToken = pConnectionToken
        }
        );
    } 
    protected virtual Task InitializeNetworkRunnerHostMigration(NetworkRunner pRunner, HostMigrationToken pHostMigrationToken)
    {
        var sceneManager = GetSceneManager(pRunner);

        pRunner.ProvideInput = true;

        return pRunner.StartGame(new StartGameArgs
        {
            SceneManager = sceneManager,
            HostMigrationToken = pHostMigrationToken,
            HostMigrationResume = HostMigrationResume
        }
        );
    }
    void HostMigrationResume(NetworkRunner pRunner)
    {
        foreach (var resumeNetworkObject in pRunner.GetResumeSnapshotNetworkObjects())
        {
            if (resumeNetworkObject.TryGetBehaviour(out Player characterController))
            {
                //pRunner.Spawn(resumeNetworkObject, position: characterController.transform.position, rotation: characterController.transform.rotation,onBeforeSpawned: (pRunner,newNetworkObject))
            }
        }
    }
    public void OnJoinLobby()
    {
        var clientTask = JoinLobby();
    }
    private async Task JoinLobby()
    {
        Debug.Log($"Join Lobby started");

        string lobbyID = "Sunken Spoils Lobby";

        var result = await networkRunner.JoinSessionLobby(SessionLobby.Custom, lobbyID);

        if (!result.Ok)
        {
            Debug.LogError($"Unable to join lobby {lobbyID}");
        }
        else
        {
            Debug.Log($"Join Lobby OK");
        }
    }
    public void CreateGame(string pSessionName, string pSceneName)
    {
        Debug.Log($"Create session {pSessionName} scene {pSceneName} build index {SceneUtility.GetBuildIndexByScenePath($"scenes/{pSceneName}")}");

        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.Host, pSessionName, NetAddress.Any(), SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath($"scenes/{pSceneName}")), 9);
    }

    public void JoinGame(SessionInfo pSessionInfo)
    {
        Debug.Log($"Join session {pSessionInfo.Name}");

        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.Client, pSessionInfo.Name, NetAddress.Any(), SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex), pSessionInfo.MaxPlayers);
    }
}
