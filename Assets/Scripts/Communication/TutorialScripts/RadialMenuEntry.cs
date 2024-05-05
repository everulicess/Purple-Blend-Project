using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
//using DG.Tweening;

public class RadialMenuEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TextMeshProUGUI Label;
    [SerializeField] RawImage Icon;

    [SerializeField]Player m_Player { get; set; }

    //RectTransform Rect;
    [SerializeField] Pings PingId;

    private void Start()
    {
        m_Player.GetComponent<PlayerCommunication>().SetPingToDisplay(Pings.None);
    }
    public void SetLabel(string pText)
    {
        Label.text = pText;
    }
    public void SetPing(Pings pPingId)
    {
        PingId = pPingId;
    } 
    public void SetPlayer(Player pPlayer)
    {
        m_Player = pPlayer;
    }
    public void SetIcon(Texture pIcon)
    {
        Icon.texture = pIcon;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        m_Player.GetComponent<PlayerCommunication>().SetPingToDisplay(PingId);

        //visual change for chosen ping
        this.transform.localScale *= 1.25f;


        ///Animation using DG.Tweening
        ///Rect.DOSComplete();
        ///Rect.DOScale(vector3.one*1.5f,3f).SetEase(Ease.OutQuad);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //visual reset for chosen ping
        this.transform.localScale /= 1.25f;
        
        //
        m_Player.GetComponent<PlayerCommunication>().SetPingToDisplay(Pings.None);

        ///Animation using DG.Tweening
        ///Rect.DOSComplete();
        ///Rect.DOScale(vector3.one,3f).SetEase(Ease.OutQuad);
    }
}
