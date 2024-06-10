using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    RectTransform rectTransform;
    float scale = 1.25f;
    Vector3 scaleVector = new(1, 1, 1);
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
        rectTransform.localScale = scaleVector;
    }
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
}
