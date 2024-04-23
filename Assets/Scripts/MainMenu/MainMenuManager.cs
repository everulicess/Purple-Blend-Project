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
    [Header("Panels")]
    [SerializeField] GameObject StartGamePanel;
    [SerializeField] GameObject SessionBrowserPanel;
    [SerializeField] GameObject statusPanel;
    [SerializeField] GameObject characterSelectionPanel;
    [SerializeField] GameObject JoinSessionPanel;

    [SerializeField] TMP_InputField sessionName;

    SessionInfo m_sessionInfo;

    public event Action<SessionInfo> OnJoinSession;
    private void Start()
    {
        HideAllPanels();
    }
    public void SetMySessionInfo(SessionInfo _sessionInfo)
    {
        m_sessionInfo = _sessionInfo;
        OnCharacterSelectionStarted();
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
    void HideAllPanels()
    {
        SessionBrowserPanel.SetActive(false);
        StartGamePanel.SetActive(false);
        statusPanel.SetActive(false);
        characterSelectionPanel.SetActive(false);
        JoinSessionPanel.SetActive(false);
    }

    public void OnCreateNewGameClicked()
    {
        
        HideAllPanels();

        StartGamePanel.SetActive(true);
        characterSelectionPanel.SetActive(true);
    }
    public void OnStartNewSessionClicked()
    {
        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();

        networkRunnerHandler.CreateGame(sessionName.text, "Movement and Communication");

        HideAllPanels();

        statusPanel.SetActive(true);
    }
    public void OnCharacterSelectionStarted()
    {
        HideAllPanels();

        characterSelectionPanel.SetActive(true);
        JoinSessionPanel.SetActive(true);
    }
    public void OnJoiningServer()
    {
        HideAllPanels();

        statusPanel.SetActive(true);
    }

    public void OnCharacterSelectionDone()
    {
        OnJoinSession?.Invoke(m_sessionInfo);
    }
}
