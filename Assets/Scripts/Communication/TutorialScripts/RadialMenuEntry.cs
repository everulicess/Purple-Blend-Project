using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Fusion;
//using DG.Tweening;

public class RadialMenuEntry : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void RadialMenuEntryDelegate(/*RadialMenuEntry pEntry,*/ Pings pPingId);
    [SerializeField] TextMeshProUGUI Label;
    [SerializeField] RawImage Icon;

    RectTransform Rect;
    RadialMenuEntryDelegate Callback;
    [SerializeField] Pings PingId;

    NetworkRunner m_Runner;
    private void Awake()
    {
        m_Runner = FindObjectOfType<NetworkRunner>();
    }
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
        //communication.PlacePing_RPC(this, this.PingId);
        //Callback?.Invoke(/*this.gameObject.GetComponent<RadialMenuEntry>(),*/ PingId);
    }
    Player m_Player;
    public void OnPointerEnter(PointerEventData eventData)
    {
        m_Player = FindObjectOfType<Player>();

        m_Player.GetComponent<PlayerCommunication>().SetPingToDisplay(PingId);
        Debug.Log($"passing the ping: {PingId}");
        if (m_Player.HasInputAuthority)
        {
            m_Player.GetComponent<PlayerCommunication>().SetPingToDisplay(PingId);
            Debug.Log($"passing the ping: {PingId}");
        }
        //FindObjectOfType<PlayerCommunication>().SetPingToDisplay(PingId);
        //Debug.LogError($"PingID is: {PingId}");
        ///Animation using DG.Tweening
        ///Rect.DOSComplete();
        ///Rect.DOScale(vector3.one*1.5f,3f).SetEase(Ease.OutQuad);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        FindObjectOfType<PlayerCommunication>().SetPingToDisplay(PingId);
        ///Animation using DG.Tweening
        ///Rect.DOSComplete();
        ///Rect.DOScale(vector3.one,3f).SetEase(Ease.OutQuad);
    }
}
