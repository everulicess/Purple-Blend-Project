using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    RectTransform rectTransform;
    float scale = 1.25f;

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
        rectTransform.localScale /= scale;
    }
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
}
