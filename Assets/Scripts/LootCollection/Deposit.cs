using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class Deposit : NetworkBehaviour
{
    [Networked] float globalGold { get; set; }
    TextMeshProUGUI TMP_GlobalGold;

    ChangeDetector changes;

    public override void Spawned()
    {
        TMP_GlobalGold = GameObject.Find("Gold Collected").GetComponentInChildren<TextMeshProUGUI>();
        TMP_GlobalGold.text = globalGold.ToString();
        changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }
    public override void Render()
    {
        UpdateGlobalGold(0f);
        foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(globalGold):
                    var floatReader = GetPropertyReader<float>(nameof(globalGold));
                    var (previousFloat, currentFloat) = floatReader.Read(previousBuffer, currentBuffer);
                    OnGoldUpdated(previousFloat, currentFloat);
                    break;
                default:
                    break;
            }
        }
    }
    private void OnGoldUpdated(float previousValue, float currentValue)
    {
        if (previousValue !> currentValue)
            return;

        TMP_GlobalGold.text = globalGold.ToString();
    }

    public void UpdateGlobalGold(float pAmountToIncrease)
    {
        globalGold += pAmountToIncrease;
    }
}
