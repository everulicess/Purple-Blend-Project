using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Fusion;
//using DG.Tweening;

public class RadialMenuEntry : NetworkBehaviour, IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    public delegate void RadialMenuEntryDelegate(/*NetworkRunner pRunner,*/RadialMenuEntry pEntry);
    [SerializeField] TextMeshProUGUI Label;
    [SerializeField] RawImage Icon;

    RectTransform Rect;
    RadialMenuEntryDelegate Callback;
    [SerializeField]Pings PingId;
    private void Start()
    {
        Rect = Icon.GetComponent<RectTransform>();
    }
    public void SetLabel(string pText)
    {
        Label.text = pText;
    }
    public void SetPing(Pings pPingId)
    {
        PingId = pPingId;
    }
    public Pings GetPingID()
    {
        return PingId;
    }
    public Texture GetIcon()
    {
        return (Icon.texture);
    }
    public void SetIcon(Texture pIcon)
    {
        Icon.texture = pIcon;
    }
    public void SetCallback(RadialMenuEntryDelegate pCallback)
    {
        Callback = pCallback;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Callback?.Invoke(/*Runner,*/this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ///Animation using DG.Tweening
        ///Rect.DOSComplete();
        ///Rect.DOScale(vector3.one*1.5f,3f).SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ///Animation using DG.Tweening
        ///Rect.DOSComplete();
        ///Rect.DOScale(vector3.one,3f).SetEase(Ease.OutQuad);
    }
}
