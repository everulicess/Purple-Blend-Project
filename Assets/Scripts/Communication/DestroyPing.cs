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
    // Update is called once per frame
    public override void Spawned()
    {
        base.Spawned();
        //locationText = GameObject.FindGameObjectWithTag("textPos").GetComponent<TextMeshProUGUI>();
        ThisObject = GetComponent<NetworkObject>();
    }
    public void Init()
    {
        destroyingTime = 3f;
    }
    public override void FixedUpdateNetwork()
    {
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
