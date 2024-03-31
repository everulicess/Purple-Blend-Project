using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

public class DestroyPing : NetworkBehaviour
{
    TickTimer life;
    [SerializeField] float destroyingTime;
    NetworkObject ThisObject;
    TextMeshProUGUI locationText;
    private bool deletinItem = false;
    Pings myPing;
    // Update is called once per frame
    public override void Spawned()
    {
        //locationText = GameObject.FindGameObjectWithTag("textPos").GetComponent<TextMeshProUGUI>();
        ThisObject = GetComponent<NetworkObject>();
    }
    public void SetPing(Pings pPingID)
    {
        //Debug.LogError($"Ping spawned with the the next ID: {pPingID}");
        myPing = pPingID;
    }
    public void Init()
    {
        destroyingTime = 3f;
    }
    public override void FixedUpdateNetwork()
    {
        name = myPing.ToString();
        //locationText.text = $"position: {transform.position}";
        if (destroyingTime < 0)
        {
            Runner.Despawn(ThisObject);
            //Destroy(this.gameObject);
        }
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))

        deletinItem = !deletinItem;

        destroyingTime -= deletinItem ? Runner.DeltaTime : 0f;

    }
}
