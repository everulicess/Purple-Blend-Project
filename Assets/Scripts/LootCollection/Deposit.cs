using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class Deposit : NetworkBehaviour
{
    [Networked] float globalGold { get; set; }
    private TextMeshProUGUI TMP_GlobalGold;
    private TextMeshProUGUI TMP_GoldPercentage;
    private float goldPercentage;
    private float totalMapGold;
    private MapManager mapManager;

    ChangeDetector changes;

    public override void Spawned()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        TMP_GlobalGold = GameObject.Find("Gold Collected").GetComponentInChildren<TextMeshProUGUI>();
        TMP_GlobalGold.text = globalGold.ToString();
        TMP_GoldPercentage = GameObject.Find("Gold Percentage").GetComponent<TextMeshProUGUI>();
        TMP_GoldPercentage.text = goldPercentage.ToString() + "%";
        totalMapGold = mapManager.totalGold;
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
        TMP_GoldPercentage.text = goldPercentage.ToString("0.0") + "%";
    }

    public void UpdateGlobalGold(float pAmountToIncrease)
    {
        totalMapGold = mapManager.totalGold;
        globalGold += pAmountToIncrease;
        goldPercentage = (globalGold / totalMapGold) * 100;
        if (goldPercentage >= 50)
        {
            mapManager.canGameEnd = true;
        }
    }
}
