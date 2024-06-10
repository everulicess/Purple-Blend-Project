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
    Button[] buttons;
    public void SetMenuInteraction(bool _interaction)
    {
        interactWithMenu = _interaction;
    }
    void Start()
    {
        MenuPanel.SetActive(false);
        buttons = MenuPanel.GetComponentsInChildren<Button>();

    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            MenuToggle();
    }
    public void MenuToggle()
    {
        foreach (Button item in buttons)
        {
            item.GetComponent<RectTransform>().localScale = new(1,1,1);
           
        }
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
