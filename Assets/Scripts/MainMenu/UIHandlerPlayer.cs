using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class UIHandlerPlayer : NetworkBehaviour
{
    private void Update()
    {
        if (Player.Local)
        {
            pocketHUDUI.SetActive(HasInputAuthority);
            pocketInGameUI.SetActive(!HasInputAuthority);

            //Relic Spots
            relictHUDUI.SetActive(HasInputAuthority);
            relicInGameUI.SetActive(!HasInputAuthority);
        }
        else
        {
            pocketHUDUI.SetActive(false);
            pocketInGameUI.SetActive(true);

            relictHUDUI.SetActive(false);
            relicInGameUI.SetActive(true);
        }
        pocketInGameUI.transform.rotation = Quaternion.Euler(30, 45, 0);
        relicInGameUI.transform.rotation = Quaternion.Euler(30, 45, 0);

    }
    #region COLLECT_UI
    [Header("User Interface")]
    [Header("Coins Pocket")]
    [SerializeField] GameObject pocketInGameUI;
    [SerializeField] GameObject pocketHUDUI;
    [SerializeField] Image[] currentPocketBar;
    [Header("Relic Spots")]
    [SerializeField] GameObject relicInGameUI;
    [SerializeField] GameObject relictHUDUI;
    Image[] relicSpotsHUD;
    Image[] relicSpotsInGame;
    [Header("Interact")]
    [SerializeField] GameObject InteractUI;

    private void ResetRelicsUI()
    {
        foreach (Image image in relicSpotsHUD)
            image.color = Color.white;

        foreach (Image image in relicSpotsInGame)
            image.color = Color.white;
    }

    private void RelicsUIUpdate(int carriedRelics)
    {
        for (int image = 0; image < carriedRelics; image++)
        {
            relicSpotsHUD[image].color = Color.yellow;
            relicSpotsInGame[image].color = Color.yellow;
        }
    }
    
    #endregion

    public override void Spawned()
    {
        relicSpotsHUD = relictHUDUI.GetComponentsInChildren<Image>();
        relicSpotsInGame = relicInGameUI.GetComponentsInChildren<Image>();

    }
}
