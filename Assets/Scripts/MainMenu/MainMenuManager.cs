using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Fusion;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject StartGamePanel;
    [SerializeField] GameObject SessionBrowserPanel;
    [SerializeField] GameObject statusPanel;

    [SerializeField] TMP_InputField sessionName;

    public void ToggleStartGameObject()
    {
        if (sessionName.text != string.Empty)
        {
            sessionName.text = " ";
        }
        StartGamePanel.SetActive(!StartGamePanel.activeSelf);
    }
    public void StartSession()
    {
        if (sessionName.text == string.Empty) return;
        Debug.Log($"{sessionName.text}session initiated");
    }
   public void OnFindGameClicked()
    {

        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();

        networkRunnerHandler.OnJoinLobby();

        HideAllPanels();

        SessionBrowserPanel.SetActive(true);
        FindObjectOfType<SessionListUIHandler>(true).OnLookingForGameSessions();

    }
    void HideAllPanels()
    {
        SessionBrowserPanel.SetActive(false);
        StartGamePanel.SetActive(false);
        statusPanel.SetActive(false);
    }

    public void OnCreateNewGameClicked()
    {
        
        HideAllPanels();

        StartGamePanel.SetActive(true);
    }
    public void OnStartNewSessionClicked()
    {
        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();

        networkRunnerHandler.CreateGame(sessionName.text, "Movement and Communication");

        HideAllPanels();

        statusPanel.SetActive(true);
    }
    public void OnJoinningServer()
    {
        HideAllPanels();

        statusPanel.SetActive(true);
    }
}
