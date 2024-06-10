using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class Deposit : NetworkBehaviour
{
    [Networked] float globalGold { get; set; }
    private TextMeshProUGUI TMP_GlobalGold;
    private float goldGoal;
    private float totalMapGold;
    private MapManager mapManager;

    ChangeDetector changes;

    public override void Spawned()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        TMP_GlobalGold = GameObject.Find("Gold Collected").GetComponentInChildren<TextMeshProUGUI>();
        totalMapGold = mapManager.totalGold;
        goldGoal = totalMapGold / 2;
        TMP_GlobalGold.text = $"{globalGold}/{goldGoal}";
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

        TMP_GlobalGold.text = $"{globalGold}/{goldGoal}";

    }
    public void UpdateGlobalGold(float pAmountToIncrease)
    {
        totalMapGold = mapManager.totalGold;
        globalGold += pAmountToIncrease;
        goldGoal = totalMapGold / 2;
        if (globalGold >= goldGoal)
        {
            mapManager.canGameEnd = true;
        }
    }

    public void UpdateTotalMapGold()
    {
        TMP_GlobalGold.text = $"{globalGold}/{goldGoal}";
    }
}
