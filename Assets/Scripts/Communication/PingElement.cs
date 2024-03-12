using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PingElement : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public Pings pingID;
    public UnityAction<Pings> SelectedPingEvent;

    private void Start()
    {
        this.gameObject.name = pingID.ToString();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Selected(pingID);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Selected(Pings.None);
    }

    public void Selected(Pings _pingID)
    {
        SelectedPingEvent.Invoke(_pingID);
        Debug.LogWarning($"Event fired,\n Event Name: {_pingID}");
    }
}
