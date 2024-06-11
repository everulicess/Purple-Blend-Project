using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonHoverBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    RectTransform rectTransform;
    float scale = 1.25f;
    Vector3 scaleVector;
    public void OnPointerClick(PointerEventData eventData)
    {
        rectTransform.localScale = scaleVector;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.localScale *= scale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.localScale = scaleVector;
    }
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        scaleVector = rectTransform.localScale;

    }
}
