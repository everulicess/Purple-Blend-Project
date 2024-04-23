using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class ClassSelectionHandler : MonoBehaviour
{
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
        characters = Enum.GetValues(typeof(Characters)).Cast<Characters>().ToList();
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
                TheMule.gameObject.SetActive(true);
                PlayerPrefs.DeleteKey("Character");
                PlayerPrefs.SetString("Character", nameof(Characters.TheMule));

                break;
            case 1:
                HideAllCharacters();
                TheBoomstick.gameObject.SetActive(true);
                PlayerPrefs.DeleteKey("Character");
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
        {
            item.SetActive(false);
        }
        ShowCharacter();
    }
}
