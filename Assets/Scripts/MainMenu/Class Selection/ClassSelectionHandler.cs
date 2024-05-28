using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Fusion;

public class ClassSelectionHandler : MonoBehaviour
{
    public static UnityAction<Characters> OnCharacterSet;
    [SerializeField] Button JoinSessionButton;
    List<Characters> characters = new();
    int currentCharacter;
    [SerializeField] GameObject[] PanelToHide;

    [SerializeField] Transform characterPosition;

    [Header("Characters")]
    [SerializeField] NetworkObject TheMule;
    [SerializeField] NetworkObject TheBoomstick;
    [SerializeField] NetworkObject TheSiren;

    private void Start()
    {
        //MainMenuManager.OnBeginGame += SetCharacterSelected;
        characters = Enum.GetValues(typeof(Characters)).Cast<Characters>().ToList();
    }

    private void SetCharacterSelected(bool arg0)
    {
        if (!arg0)
            return;
        switch (currentCharacter)
        {
            case 0:
                OnCharacterSet?.Invoke(Characters.TheMule);
                break;
            case 1:
                OnCharacterSet?.Invoke(Characters.TheBoomstick);

                break;
            case 2:
                OnCharacterSet?.Invoke(Characters.TheSiren);
                break;
            default:
                break;
        }
    }

    public void NextCharacter()
    {
        currentCharacter++;
        if (currentCharacter >= characters.Count)
        {
            currentCharacter = 0;
        }
        ShowCharacter();
    }
    private void ShowCharacter()
    {
        switch (currentCharacter)
        {
            case 0:
                HideAllCharacters();
                PlayerPrefs.DeleteKey("Character");
                TheMule.gameObject.SetActive(true);
                PlayerPrefs.SetString("Character", nameof(Characters.TheMule));
                break;
            case 1:
                HideAllCharacters();
                PlayerPrefs.DeleteKey("Character");
                TheBoomstick.gameObject.SetActive(true);
                PlayerPrefs.SetString("Character", nameof(Characters.TheBoomstick));
                break;
            case 2:
                HideAllCharacters();
                TheSiren.gameObject.SetActive(true);
                PlayerPrefs.DeleteKey("Character");
                PlayerPrefs.SetString("Character", nameof(Characters.TheSiren));
                break;
            default:
                break;
        }
        PlayerPrefs.Save();
    }
    private void HideAllCharacters()
    {
        TheMule.gameObject.SetActive(false);
        TheBoomstick.gameObject.SetActive(false);
        TheSiren.gameObject.SetActive(false);
    }
    public void PreviousCharacter()
    {
        currentCharacter--;
        if (currentCharacter < 0)
        {
            currentCharacter = characters.Count - 1;
        }
        ShowCharacter();
    }
    private void OnEnable()
    {
        foreach (GameObject item in PanelToHide)
            item.SetActive(false);
        ShowCharacter();
    }
}
