using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

public class DestroyPing : NetworkBehaviour
{
    [SerializeField] float destroyingTime;
    [SerializeField] NetworkObject ThisObject;
    TextMeshProUGUI locationText;
    // Update is called once per frame
    public override void Spawned()
    {
        locationText = GameObject.FindGameObjectWithTag("textPos").GetComponent<TextMeshProUGUI>();
    }
    public override void FixedUpdateNetwork()
    {
        locationText.text = $"position: {transform.position}";
        if (destroyingTime < 0)
        {
            Runner.Despawn(ThisObject);
            //Destroy(this.gameObject);
        }
    }
    void Update()
    {
        destroyingTime -= Time.deltaTime;

    }
}
