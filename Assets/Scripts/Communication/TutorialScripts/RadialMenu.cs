using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Fusion;

public class RadialMenu : NetworkBehaviour
{
    [SerializeField] GameObject EntryPrefab;

    List<RadialMenuEntry> Entries;

    [SerializeField] List<Texture> Icons;
    float Radius = 75f;

    [SerializeField] RawImage TargetIcon;
    // Start is called before the first frame update
    void Start()
    {
        Entries = new();
    }

    void AddEntry(Pings pPingId, RadialMenuEntry.RadialMenuEntryDelegate pCallback)
    {
        //Get references for each ping
        CommunicationManager.IconsDictionary.TryGetValue(pPingId, out Texture icon);
        GameObject entry = Instantiate(EntryPrefab, transform);

        RadialMenuEntry rme = entry.GetComponent<RadialMenuEntry>();
        rme.SetLabel(pPingId.ToString());
        rme.SetIcon(icon);
        rme.SetCallback(pCallback);
        rme.SetPing(pPingId);

        Entries.Add(rme);
    }
    public void Open()
    {
        if (Entries.Count != 0) return;
        foreach (Pings ping in Enum.GetValues(typeof(Pings)))
        {
            AddEntry(ping, PlacePing_RPC);
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
    //public void Toggle()
    //{
    //    if (Entries.Count==0)
    //    {
    //        Open();
    //    }
    //    else
    //    {
    //        Close();
    //    }
    //}
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
    //void SetTargetIcon(RadialMenuEntry pEntry)
    //{
    //    TargetIcon.texture = pEntry.GetIcon();
    //}
    //NetworkRunner runner;
    [Rpc(RpcSources.All,RpcTargets.StateAuthority)]
    public void PlacePing_RPC(/*NetworkRunner prunner,*/ RadialMenuEntry pEntry)
    {
        //if (!HasInputAuthority) return;
        //if (runner) return;
        Debug.LogError($"can send the RPC");
        Pings _ping = pEntry.GetPingID();
        //get sound and visuals for each ping
        CommunicationManager.visualsDictionary.TryGetValue(_ping, out GameObject _pingVisual);
        CommunicationManager.audioDictionary.TryGetValue(_ping, out AudioClip _pingSound);
        Vector3 pos = CommunicationManager.pingMenuPosition;
        Camera cam = FindObjectOfType<Camera>();
        Ray ray = cam.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject.layer == 8)
            {
                Vector3 offset = new(hit.point.x, hit.point.y + 0.1f, hit.point.z);
                Runner.Spawn(_pingVisual, offset, Quaternion.identity);
                AudioSource.PlayClipAtPoint(_pingSound, offset);

                Debug.Log($"{_pingVisual.name} has been placed and {_pingSound.name} is ebing played");
            }
        }
    }
}
