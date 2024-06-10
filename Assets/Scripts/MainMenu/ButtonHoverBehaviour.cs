using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonHoverBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    RectTransform rectTransform;
    float scale = 1.25f;
    [SerializeField] Vector3 scaleVector;
    public void OnPointerClick(PointerEventData eventData)
    {
        rectTransform.localScale /= scale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.localScale *= scale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (SceneManager.GetActiveScene().name == "MenuScene")
        {
            rectTransform.localScale = scaleVector;
        }
        else
        {
            rectTransform.localScale = new(1, 1, 1);
        }
    }
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        scaleVector = rectTransform.localScale;

    }
}
