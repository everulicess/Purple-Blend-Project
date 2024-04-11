using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Fusion;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject StartGameUIObject;
    [SerializeField] TMP_InputField sessionName;

    public void ToggleStartGameObject()
    {
        if (sessionName.text != string.Empty)
        {
            sessionName.text = " ";
        }
        StartGameUIObject.SetActive(!StartGameUIObject.activeSelf);
    }
    public void StartSession()
    {
        if (sessionName.text == string.Empty) return;
        Debug.Log($"{sessionName.text}session initiated");
        GameStart(GameMode.AutoHostOrClient, sessionName.text);
    }
    NetworkRunner networkRunner;
    async void GameStart(GameMode pMode, string pSessionName)
    {
        networkRunner = gameObject.AddComponent<NetworkRunner>();
        networkRunner.ProvideInput = true;

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();

        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = pMode,
            SessionName = pSessionName,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            //PlayerCount = 2
        });
        SceneManager.LoadScene(2);
    }
}
