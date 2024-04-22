using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;
using System;

public class SessionListUIHandler : MonoBehaviour
{
    public TextMeshProUGUI StatusText;

    public GameObject SessionItemListPrefab;
    public VerticalLayoutGroup VerticalLayoutGroup;
    
    private void Awake()
    {
        ClearList();
    }
    public void ClearList()
    {
        foreach (Transform child in VerticalLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }

        StatusText.gameObject.SetActive(false);
    }

    public void AddToList(SessionInfo pSessionInfo)
    {
        SessionInfoListUIItem addedSessionInfoUIItem = Instantiate(SessionItemListPrefab, VerticalLayoutGroup.transform).GetComponent<SessionInfoListUIItem>();

        addedSessionInfoUIItem.SetInformation(pSessionInfo);

        MainMenuManager mainMenu = FindObjectOfType<MainMenuManager>();
        mainMenu.OnJoinSession += AddedSessionInfoListUIItem_OnJoinSession;

    }

    private void AddedSessionInfoListUIItem_OnJoinSession(SessionInfo obj)
    {
        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();
        networkRunnerHandler.JoinGame(obj);

        MainMenuManager mainMenuManager = FindObjectOfType<MainMenuManager>();
        mainMenuManager.OnJoiningServer();
    }

    public void OnNoSessionFound()
    {
        ClearList();
        StatusText.text = "NO GAME SESSION FOUND";
        StatusText.gameObject.SetActive(true);
    }
    public void OnLookingForGameSessions()
    {
        ClearList();
        StatusText.text = "LOOKING FOR GAME SESSION";
        StatusText.gameObject.SetActive(true);
    }

}
