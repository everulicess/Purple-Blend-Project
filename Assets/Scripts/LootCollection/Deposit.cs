using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class Deposit : NetworkBehaviour
{
    [Networked] float globalGold { get; set; }
    [SerializeField] TextMeshProUGUI TMP_GlobalGold;
  
    public override void Render()
    {
        UpdateGlobalGold_RPC(0f);
    }
    public override void Spawned()
    {
        TMP_GlobalGold.text = globalGold.ToString();
    }
    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
   public void UpdateGlobalGold_RPC(float pAmountToIncrease) 
    {
        UpdateGlobalGoldForClients_RPC(pAmountToIncrease);
    }
    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void UpdateGlobalGoldForClients_RPC(float pAmountToIncrease)
    {
        globalGold += pAmountToIncrease;
        TMP_GlobalGold.text = globalGold.ToString();
    }
}
