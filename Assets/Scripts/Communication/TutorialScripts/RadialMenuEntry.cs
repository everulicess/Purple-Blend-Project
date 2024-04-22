using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
//using DG.Tweening;

public class RadialMenuEntry : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TextMeshProUGUI Label;
    [SerializeField] RawImage Icon;

    Player m_Player;

    //RectTransform Rect;
    [SerializeField] Pings PingId;

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
    public void OnPointerClick(PointerEventData eventData)
    {
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        m_Player = FindObjectOfType<Player>();

        m_Player.GetComponent<PlayerCommunication>().SetPingToDisplay(PingId);
        //Debug.Log($"passing the ping: {PingId}");
        
        ///Animation using DG.Tweening
        ///Rect.DOSComplete();
        ///Rect.DOScale(vector3.one*1.5f,3f).SetEase(Ease.OutQuad);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //FindObjectOfType<PlayerCommunication>().SetPingToDisplay(PingId);
        ///Animation using DG.Tweening
        ///Rect.DOSComplete();
        ///Rect.DOScale(vector3.one,3f).SetEase(Ease.OutQuad);
    }
}
