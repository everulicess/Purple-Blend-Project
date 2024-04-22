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
    [SerializeField] List<Characters> characters = new();
    GameObject currentObject;
    int currentCharacter;
    [SerializeField] GameObject[] thingsToHide;

    [SerializeField] Transform characterPosition;

    [Header("Characters")]
    [SerializeField] NetworkObject TheMule;
    [SerializeField] NetworkObject TheBoomstick;
    [SerializeField] NetworkObject TheSiren;

    private void Start()
    {
        //JoinSessionButton.enabled = false;
        characters = Enum.GetValues(typeof(Characters)).Cast<Characters>().ToList();
    }
    public void NextCharacter()
    {
        //characters[currentCharacter].gameObject.SetActive(false);
        currentCharacter++;
        if (currentCharacter >= characters.Count)
        {
            currentCharacter = 0;
        }
        ShowCharacter();
        //characters[currentCharacter].gameObject.SetActive(true);
        //ShowCharacter();
    }

    private void ShowCharacter()
    {
        switch (currentCharacter)
        {
            case 0:
                HideAllCharacters();
                TheMule.gameObject.SetActive(true);
                PlayerPrefs.DeleteKey("Character");
                PlayerPrefs.SetString("Character", Characters.TheMule.ToString());

                break;
            case 1:
                HideAllCharacters();
                TheBoomstick.gameObject.SetActive(true);
                PlayerPrefs.DeleteKey("Character");
                PlayerPrefs.SetString("Character", Characters.TheBoomstick.ToString());
                break;
            case 2:
                HideAllCharacters();
                TheSiren.gameObject.SetActive(true);
                PlayerPrefs.DeleteKey("Character");
                PlayerPrefs.SetString("Character", Characters.TheSiren.ToString());
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
        //characters[currentCharacter].gameObject.SetActive(false);
        currentCharacter--;
        if (currentCharacter < 0)
        {
            currentCharacter = characters.Count - 1;
        }
        ShowCharacter();
        //characters[currentCharacter].gameObject.SetActive(true);

        //ShowCharacter();
    }
    public void OnCharacterSelected()
    {
        JoinSessionButton.enabled = true;
    }
    private void OnEnable()
    {
        foreach (GameObject item in thingsToHide)
        {
            item.SetActive(false);
        }  
    }
}
