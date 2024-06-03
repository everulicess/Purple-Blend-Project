using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Fusion;

public class InGameMenu : MonoBehaviour
{
    [Header("Panel reference")]
    [SerializeField] GameObject MenuPanel;
    [SerializeField] Slider VolumeSlider;
    [Header("Settings")]
    [SerializeField] int MenuScene;

    bool interactWithMenu;
    NetworkRunner Runner;
    
    public void SetMenuInteraction(bool _interaction)
    {
        interactWithMenu = _interaction;
    }
    void Start()
    {
        MenuPanel.SetActive(false);
    }

    private void LateUpdate()
    {
        if (interactWithMenu)
            MenuToggle();
    }
    public void MenuToggle()
    {
        if (Player.Local)
        {
            MenuPanel.SetActive(!MenuPanel.activeInHierarchy);
        }
        else
        {
            MenuPanel.SetActive(false);

        }
    }
    public void OnVolumeChanged()
    {
        Debug.Log("volume changed");
    }
    public void OnQuitGame()
    {
        Application.Quit();
    }
    public void OnQuitGameSession()
    {
        Runner = FindObjectOfType<NetworkRunner>();
        Runner.Shutdown();
        SceneManager.LoadScene(MenuScene);
    }
}
