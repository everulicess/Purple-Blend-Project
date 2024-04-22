using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class Collector : NetworkBehaviour
{
    //UI
    [SerializeField] GameObject pocketUI;
    [SerializeField] Image totalPocketBar;
    [SerializeField] Image currentPocketBar;
    [Networked] float currentFill { get; set; }

    //Storage variables
    ChangeDetector _changes;

    //Coins
    [Range(1f, 500f)]
    [SerializeField] float pocketCapacity;
    [Networked] float CarriedPocketLoot { get; set; }
    int CollectedCoins;
    readonly float coinsValue = 25f;

    //Relics
    [Range(1, 4)]
    [SerializeField] int relicCapacity;
    int carriedRelics;
    readonly float relicValue = 50f;

    //Treasure
    bool carryingTreasure = false;
    [SerializeField] Transform carryPoint;
    GameObject treasure;

    //Total player's gold
    float totalPlayerGold = 0f;
    NetworkObject net_objectPickedup;
    bool isInteracting;

    public override void Spawned()
    {
        _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }
    private void Update()
    {
        if (HasInputAuthority)
        {
            pocketUI.SetActive(true);
            pocketUI.transform.rotation = Quaternion.Euler(30, 45, 0);

            currentFill = CarriedPocketLoot;
            currentPocketBar.fillAmount = currentFill / pocketCapacity;
        }
        else
        {
            pocketUI.SetActive(false);
        }
    }
    public bool GetCarryingBool()
    {
        return carryingTreasure;
    }
    private static Collider[] _colliders = new Collider[20];
    [SerializeField] LayerMask LayerMask;
    public override void FixedUpdateNetwork()
    {
        int collisions = Runner.GetPhysicsScene().OverlapSphere(new Vector3(transform.position.x,0,transform.position.z), 1f, _colliders, LayerMask, QueryTriggerInteraction.Collide);
        for (int i = 0; i < collisions; i++)
        {
            // Check for Collectable component on collider game object or any parent.
            _colliders[i].TryGetComponent(out Collectable _collectable);
            var collectable = _collectable;
            if (collectable != null)
            {
                collectable.TryInteracting(this);
            }
        }
    }
    public override void Render()
    {
        foreach (var change in _changes.DetectChanges(this, out var previousBuffer,out var currentBuffer))
        {
            switch (change)
            {
                case nameof(CarriedPocketLoot):
                    var reader = GetPropertyReader<float>(nameof(CarriedPocketLoot));
                    var (previous, current) = reader.Read(previousBuffer, currentBuffer);
                    OnCarriedPocketChange(previous, current)
                    ;break;
                default:
                    break;
            }
        }
    }

    private void OnCarriedPocketChange(float previous, float current)
    {
        if (previous < current)
        {
            Debug.Log("Increasing Gold");
        }
        else
        {
            Debug.Log("Decreasing Gold");

        }
    }

    public void SetInteractionBool(bool pIsInteracting)
    {
        isInteracting = pIsInteracting;
    }
    public void SetObjectToPickUp(NetworkObject pObjectToPickup)
    {
        net_objectPickedup = pObjectToPickup;
    }
    /// <summary>
    /// Checks if the player has enough space for more gold, then picks it up
    /// </summary>
    /// <param name="pCoin"></param> reference to the coin script
    /// <param name="amountToIncrease"></param> increasing amount
    public void CollectCoins(Collectable pCoin, float amountToIncrease)
    {
        if (CarriedPocketLoot > (pocketCapacity-amountToIncrease)) return;
        pCoin.DeleteObject();
        CollectedCoins++;
        CarriedPocketLoot = CollectedCoins * coinsValue;
        totalPlayerGold = CarriedPocketLoot + (carriedRelics*relicValue);
        CarriedPocketLoot = Mathf.Clamp(CarriedPocketLoot, 0f, pocketCapacity);
        Debug.Log($"collected coins: {CollectedCoins}");
    }
    /// <summary>
    /// Checks if the player is trying to interact and if there is anough space for the relic
    /// </summary>
    /// <param name="pRelic"></param> refrence to the relic, to destroy it if it's collected
    public void CollectRelic(Collectable pRelic, float pAmountToIncrease)
    {
        if (isInteracting)
        {
            //If the player has a relic slot left
            if (carriedRelics >= relicCapacity)
            {
                Debug.Log("Your relic slots are full!");
                return;
            }
            carriedRelics++;
            totalPlayerGold = (carriedRelics * relicValue);
            //totalPlayerGold += pAmountToIncrease;
            pRelic.DeleteObject();
        }
    }
    public bool CanPickUp()
    {
        return !carryingTreasure;
    }
    public void CollectTreasure(Collectable pTreasure, Rigidbody pRigidBody, BoxCollider pCollider)
    {
        if (isInteracting)
        {
            carryingTreasure = !carryingTreasure;
            pRigidBody.useGravity = !carryingTreasure;
            pRigidBody.isKinematic = !carryingTreasure;
            pCollider.enabled = !carryingTreasure;
        }
        //treasure = carryingTreasure ? pTreasure.gameObject : null;

        if (carryingTreasure)
        {
            treasure = net_objectPickedup.gameObject;
            treasure.transform.SetPositionAndRotation(carryPoint.position, carryPoint.rotation);
            //pTreasure.transform.position = carryPoint.position;
        }
    }
    bool hasEntered1 = true;

    private void OnTriggerStay(Collider other)
    {
        hasEntered1 = true;
        other.TryGetComponent(out Collectable _collectable);
        if (_collectable != null)
        {
            if (hasEntered1 == false) return;
            if (hasEntered1)
            {
                //Debug.LogError($"HAS {other.name} ENTERED? {hasEntered1} and this player has: {CarriedPocketLoot} in his pockets");
                _collectable.TryInteracting(this);
                hasEntered1 = false;
            }
        }

        other.TryGetComponent(out Deposit _deposit);
        if (_deposit != null)
        {
            bool hasEntered = true;
            if (hasEntered)
            {
                if(totalPlayerGold <= 0) return;
                CarriedPocketLoot = 0;
                carriedRelics = 0;
                CollectedCoins = 0;
                _deposit.UpdateGlobalGold_RPC(totalPlayerGold);
                totalPlayerGold = 0;
                hasEntered = false;
            }
            else
            {
                return;
            }
        }
    }
    private void Pickup()
    {
        GameObject _cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _cube.AddComponent<NetworkObject>();
        
        Runner.Spawn(_cube, this.gameObject.transform.position + (Vector3.forward * 3f), Quaternion.identity);
        if (treasure == null) return;
        treasure.transform.SetPositionAndRotation(carryPoint.position, carryPoint.rotation);
    }
}
