using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
public class Health : NetworkBehaviour, IDamageable
{
    [Header("Health User Interface")]
    //UI
    [SerializeField] GameObject HealthInGameUI;
    [SerializeField] GameObject HealthHUDUI;
    [SerializeField] Image[] fillImages;

    [Header("change detectors")]
    //change Detector
    private ChangeDetector _changes;
    [Networked] byte HealthPoints { get; set; }
    [Networked] bool isDead { get; set; }

    const float reviveTime = 5f;
    float currentReviveTime;
    [SerializeField] byte maxHealthPoints = 100;

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
            image.fillAmount = HealthPoints / maxHealthPoints;
        }
    }
    private void Update()
    {
        //if (!isInitailized)
        //    return;
        //if (isDead)
        //    return;
        HealthUIHandle();
    }

    private void HealthUIHandle()
    {
        if (isPlayer)
        {
            if (Player.Local)
            {
                HealthHUDUI.SetActive(HasInputAuthority);
                HealthInGameUI.SetActive(!HasInputAuthority);
            }
            else
            {
                HealthHUDUI.SetActive(false);
                HealthInGameUI.SetActive(true);
            }
        }
        else
        {
            HealthHUDUI.SetActive(false);
        }
        HealthInGameUI.transform.rotation = Quaternion.Euler(30, 45, 0);
    }

    public override void FixedUpdateNetwork()
    {
        if (currentReviveTime >= 0)
        {
            currentReviveTime -= Time.deltaTime;
            if (currentReviveTime < 0)
                isDead = false;
        }

        //UpdateHealthBar();
    }
    public override void Render()
    {
        foreach (var change in _changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(HealthPoints):
                    var byteReader = GetPropertyReader<byte>(nameof(HealthPoints));
                    var (previousByte, currentByte) = byteReader.Read(previousBuffer, currentBuffer);
                    OnHealthChanged(previousByte, currentByte);
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
        if (isPlayer)
        {
            model.GetComponentInParent<Player>().enabled = false;
        }
        else
        {
            model.GetComponentInParent<BaseEnemy>().enabled = false;
            //Runner.Despawn(this.gameObject.GetComponent<NetworkObject>());
        }
        HealthInGameUI.SetActive(false);
        model.SetActive(false);

        currentReviveTime = reviveTime;
    }

    private void OnRevive()
    {
        if (isPlayer)
        {
            model.GetComponentInParent<Player>().enabled = true;
            model.GetComponentInParent<Player>().OnRespawn();
        }
        else
        {
            model.GetComponentInParent<BaseEnemy>().enabled = true;
        }
        HealthPoints = maxHealthPoints;
        HealthInGameUI.SetActive(true);
        model.SetActive(true);
    }
    void OnHealthChanged(byte oldValue, byte value)
    {
        if (value < oldValue)
            OnHPReduced();
    }
    public void OnTakeDamage(byte pDamage)
    {
        if (isDead) 
            return;

        if (pDamage > HealthPoints)
            pDamage = HealthPoints;
        
        HealthPoints -= pDamage;

        if (HealthPoints <= 0)
        {
            isDead = true;
        }
    }
    public void OnHPReduced()
    {
        if (!isInitailized) 
            return;
        UpdateHealthBar();
    }
    private void OnHPIncreased()
    {

    }
    
}
