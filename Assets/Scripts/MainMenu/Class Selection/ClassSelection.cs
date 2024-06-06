using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Fusion;

public class ClassSelection : MonoBehaviour
{
    public static UnityAction<Characters> OnCharacterSet;
    List<Characters> characters = new();
    int currentCharacter;
    bool selectionDone { get; set; }
    [Header("Characters")]
    [SerializeField] GameObject TheMule;
    [SerializeField] GameObject TheBoomstick;
    [SerializeField] GameObject TheSiren;

    private void Start()
    {
        characters = Enum.GetValues(typeof(Characters)).Cast<Characters>().ToList();
    }
   
    public void SetCharacterSelected()
    {
        if (!NetworkPlayer.Local)
            return;
        Destroy(gameObject);
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
                TheMule.gameObject.SetActive(true);
                break;
            case 1:
                HideAllCharacters();
                TheBoomstick.gameObject.SetActive(true);
                break;
            case 2:
                HideAllCharacters();
                TheSiren.gameObject.SetActive(true);
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
        ShowCharacter();
    }
}
