using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using TMPro;
public class Health : NetworkBehaviour, IDamageable
{
    [Header("health user interface")]
    //ui
    [SerializeField] GameObject HealthInGameUI;
    [SerializeField] GameObject HealthHUDUI;
    [SerializeField] GameObject DeadUI;
    [SerializeField] Image[] fillImages;

    [Header("change detectors")]
    //change Detector
    private ChangeDetector _changes;
    [Networked] float HealthPoints { get; set; }
    [Networked] public bool isDead { get; set; }

    const float reviveTime = 5f;
    [Networked]float currentReviveTime { get; set; }
    [SerializeField] float maxHealthPoints = 100;

    bool isInitailized;
    bool isPlayer;

    [SerializeField] GameObject model;
    public override void Spawned()
    {
        _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
        HealthPoints = maxHealthPoints;
        isDead = false;
        isPlayer = this.TryGetComponent(out Player _player);
        isInitailized = true;

        HealthUIHandle();
        UpdateHealthBar();
    }
    void UpdateHealthBar()
    {
        foreach (Image image in fillImages)
        {
            image.fillAmount = (float)(HealthPoints) / (float)(maxHealthPoints);
        }
    }
    private void Update()
    {
        HealthUIHandle();
    }

    private void HealthUIHandle()
    {
        if (isPlayer)
        {
            if (Player.Local)
            {
                DeadUI.SetActive(HasInputAuthority && currentReviveTime > 0);
                HealthHUDUI.SetActive(HasInputAuthority);
                //HealthInGameUI.SetActive(!HasInputAuthority);
            }
            else
            {
                HealthHUDUI.SetActive(false);
                //HealthInGameUI.SetActive(isDead && !HasInputAuthority);
            }
            HealthInGameUI.SetActive(Player.Local? !HasInputAuthority : isDead);

        }
        else
        {
            HealthInGameUI.SetActive(true);
        }
        HealthInGameUI.transform.rotation = Quaternion.Euler(30, 45, 0);
    }

    public override void FixedUpdateNetwork()
    {
        if (!isPlayer)
            return;
        if (currentReviveTime >= 0)
        {
            DeadUI.GetComponentInChildren<TextMeshProUGUI>().text = $"Respawn in: \n {currentReviveTime.ToString("0")}";
            currentReviveTime -= Runner.DeltaTime;
            if (currentReviveTime < 0)
                isDead = false;
        }
    }
    public override void Render()
    {
        foreach (var change in _changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(HealthPoints):
                    var floatReader = GetPropertyReader<float>(nameof(HealthPoints));
                    var (previousFloat, currentFloat) = floatReader.Read(previousBuffer, currentBuffer);
                    OnHealthChanged(previousFloat, currentFloat);
                    break;
                case nameof(isDead):
                    var boolReader = GetPropertyReader<bool>(nameof(isDead));
                    var (previousBool, currentBool) = boolReader.Read(previousBuffer, currentBuffer);
                    OnBoolChanged(previousBool, currentBool);
                    ;break;
            }
        }
    }
     void OnBoolChanged(bool oldValue, bool value)
    {
        if (value)
            OnDeath();
        else if (!value && oldValue)
            OnRevive();
    }
    private void OnDeath()
    {
        if(!isPlayer)
        {
            model.GetComponentInParent<BaseEnemy>().enabled = false;
            Runner.Despawn(this.gameObject.GetComponent<NetworkObject>());
        }
        else
        {
            currentReviveTime = reviveTime;

        }
        HealthInGameUI.SetActive(false);
        model.SetActive(false);

    }

    private void OnRevive()
    {
        if (isPlayer)
        {
            model.GetComponentInParent<Player>().enabled = true;
        }
        else
        {
            model.GetComponentInParent<BaseEnemy>().enabled = true;
        }
        HealthPoints = maxHealthPoints;
        UpdateHealthBar();
        HealthInGameUI.SetActive(true);
        model.SetActive(true);
    }
    void OnHealthChanged(float oldValue, float value)
    {
        UpdateHealthBar();
        if (value < oldValue)
            OnHPReduced();
    }
    public void OnTakeDamage(float pDamage)
    {
        if (HealthPoints <= 0) 
            return;

        if (pDamage > HealthPoints)
            pDamage = HealthPoints;
        
        HealthPoints -= pDamage;

        
    }
    public void OnHPReduced()
    {
        if (!isInitailized) 
            return;
        if (HealthPoints <= 0)
        {
            isDead = true;
        }
    }
    private void OnHPIncreased()
    {

    }
    
}
