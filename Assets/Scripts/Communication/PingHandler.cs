using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

public class PingHandler : NetworkBehaviour
{
    TickTimer life;
    [SerializeField] float destroyingTime;
    NetworkObject ThisObject;
    [SerializeField] Pings myPing;

    private void Awake()
    {
        Init();
    }
    public override void Spawned()
    {
        ThisObject = GetComponent<NetworkObject>();
    }
    public void SetPing(Pings pPingID)
    {
        myPing = pPingID;
    }
    public void Init()
    {
        CommunicationManager.audioDictionary.TryGetValue(myPing, out AudioClip audio);
        if (audio == null) return;
        Debug.Log($"playing this sound: {audio}");
        AudioSource.PlayClipAtPoint(audio, transform.position);
        destroyingTime = 3f;
    }
    public override void FixedUpdateNetwork()
    {
        if (destroyingTime < 0)
        {
            Runner.Despawn(ThisObject);
        }
    }
    void Update()
    {
        destroyingTime -= Runner.DeltaTime;
    }
}
