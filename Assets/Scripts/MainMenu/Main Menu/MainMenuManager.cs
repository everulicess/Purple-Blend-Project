using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Fusion;
using System;

public class MainMenuManager : MonoBehaviour
{
    // If you create a new panel, put it here.
    [Header("Panels")]
    [SerializeField] GameObject StartGamePanel;
    [SerializeField] GameObject SessionBrowserPanel;
    [SerializeField] GameObject StatusPanel;
    [SerializeField] GameObject SettingsPanel;
    [SerializeField] GameObject MainMenuPanel;
    [SerializeField] GameObject TutorialPanel;

    [SerializeField] TMP_InputField sessionName;


    private void Start()
    {
        HideAllPanels();
        MainMenuPanel.SetActive(true);
    }
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

    // Daans work on quitting the game.
    public void OnQuitGameClicked()
    {
        Application.Quit();
    }

    public void OnTutorialClicked()
    {
        HideAllPanels();

        TutorialPanel.SetActive(true);
    }

    public void OnSettingsClicked()
    {
        HideAllPanels();
        SettingsPanel.SetActive(true);

    }

    public void MainMenu()
    {
        HideAllPanels();
        MainMenuPanel.SetActive(true);

    }

    // End of Daans work.


    // Add your panel here for deactivation
    void HideAllPanels()
    {
        SessionBrowserPanel.SetActive(false);
        StartGamePanel.SetActive(false);
        StatusPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        MainMenuPanel.SetActive(false);
        TutorialPanel.SetActive(false);
    }

    public void OnCreateNewGameClicked()
    {
        
        HideAllPanels();

        StartGamePanel.SetActive(true);
        //characterSelectionPanel.SetActive(true);
    }
    public void OnStartNewSessionClicked()
    {
        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();

        networkRunnerHandler.CreateGame(sessionName.text, "Movement and Communication");

        HideAllPanels();

        StatusPanel.SetActive(true);
        FindObjectOfType<SessionListUIHandler>(true).StatusText.text = "Loading...";

    }


    public void OnJoiningServer()
    {
        HideAllPanels();

        StatusPanel.SetActive(true);
        FindObjectOfType<SessionListUIHandler>(true).StatusText.text = "Loading...";

    }


}
