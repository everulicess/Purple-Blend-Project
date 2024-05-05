using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RadialMenu : MonoBehaviour
{
    [SerializeField] GameObject EntryPrefab;

    List<RadialMenuEntry> Entries;

    float Radius = 80f;

    [SerializeField] Player m_Player;
    private void Awake()
    {
        Entries = new();
    }
    void AddEntry(Pings pPingId, Player pPlayer)
    {
        CommunicationManager.IconsDictionary.TryGetValue(pPingId, out Texture icon);

        GameObject entry = Instantiate(EntryPrefab, transform);

        RadialMenuEntry rme = entry.GetComponent<RadialMenuEntry>();
        rme.SetPing(pPingId);
        rme.SetLabel(pPingId.ToString());
        rme.SetIcon(icon);
        rme.SetPlayer(m_Player);

        Entries.Add(rme);
    }
    public void Open()
    {
        if (Entries.Count != 0) return;
        foreach (Pings ping in Enum.GetValues(typeof(Pings)))
        {
            if (ping != Pings.None) 
                AddEntry(ping, m_Player);
        }
        Rearrange();
    }
    public void Close()
    {
        if (Entries.Count == 0) return;
        for (int i = 0; i < Entries.Count; i++)
        {
            GameObject entry = Entries[i].gameObject;
            Destroy(entry);
        }
        Entries.Clear();
    }
    void Rearrange()
    {
        float radiansOfSeparation = (Mathf.PI * 2) / Entries.Count;
        for (int i = 0; i < Entries.Count; i++)
        {
            float x = Mathf.Sin(radiansOfSeparation * i) * Radius;
            float y = Mathf.Cos(radiansOfSeparation * i) * Radius;

            Entries[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);
        }
    }
}
