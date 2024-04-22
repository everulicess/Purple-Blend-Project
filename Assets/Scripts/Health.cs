using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
public class Health : NetworkBehaviour, IDamageable
{
    [Header("Health User Interface")]
    //UI
    [SerializeField] GameObject HealthUI;
    [SerializeField] Image fillImage;

    [Header("change detectors")]
    //change Detector
    private ChangeDetector _changes;
    [Networked] float HealthPoints { get; set; }
    [Networked] bool isDead { get; set; }

    [SerializeField] float maxHealthPoints = 100f;

    bool isInitailized;
    public bool isPlayer;
    private void Start()
    {
        //    _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
        //    isDead = false;
        //    fillImage.fillAmount = HealthPoints / maxHealthPoints;
        //    isPlayer = this.TryGetComponent(out Player _player);
    }
    public override void Spawned()
    {
        _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
        HealthPoints = maxHealthPoints;
        isDead = false;
        fillImage.fillAmount = HealthPoints / maxHealthPoints;
        isPlayer = this.TryGetComponent(out Player _player);
        isInitailized = true;
    }
    private void Update()
    {
        HealthUI.SetActive(true);
        HealthUI.transform.rotation = Quaternion.Euler(30, 45, 0);
    }
    public override void FixedUpdateNetwork()
    {
        fillImage.fillAmount = HealthPoints / maxHealthPoints;
    }
    public override void Render()
    {
        if (isDead && !isPlayer)
        { 
            Runner.Despawn(this.gameObject.GetComponent<NetworkObject>());
            return;
        }

        foreach (var change in _changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(HealthPoints):
                    var reader = GetPropertyReader<float>(nameof(HealthPoints));
                    var (previous, current) = reader.Read(previousBuffer, currentBuffer);
                    OnHealthChanged(previous, current);
                    break;
                case nameof(isDead):
                    var reader2 = GetPropertyReader<bool>(nameof(isDead));
                    var (previous2, current2) = reader2.Read(previousBuffer, currentBuffer);
                    OnBoolChanged(previous2, current2);
                    ;break;
            }
        }
    }

     void OnHealthChanged(float oldValue, float value)
    {
        if (value<oldValue)
        {
            OnHPReduced();
        }
        //Debug.Log($"{Time.time} On Health Changed \n old value {oldValue} \n new value{value}");
    }
    static void OnBoolChanged(bool oldValue, bool value)
    {
        //Debug.Log($"{Time.time} On boolean Changed \n old value {oldValue} \n new value{value}");

    }
    public void OnTakeDamage(float pDamage)
    {
        if (isDead) return;
        
        HealthPoints -= pDamage;
        //Debug.Log($"{Time.deltaTime} {transform.name} took damaage and has {HealthPoints} left ");

        if (HealthPoints<=0)
        {
            isDead = true;
        }
    }
    public void OnHPReduced()
    {
        if (!isInitailized) return;
        fillImage.fillAmount = HealthPoints / maxHealthPoints;

    }
    private void OnHPIncreased()
    {

    }
    
}
