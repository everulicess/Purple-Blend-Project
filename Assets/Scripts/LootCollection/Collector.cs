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
    [Networked] float carriedPocketLoot { get; set; }

    //Relics
    [Range(1, 4)]
    [SerializeField] int relicCapacity;
    int carriedRelics;

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
            pocketUI.transform.LookAt(FindObjectOfType<Camera>().transform.position);

            currentFill = carriedPocketLoot;
            currentPocketBar.fillAmount = currentFill / pocketCapacity;
        }
        else
        {
            pocketUI.SetActive(false);
        }
    }
    private static Collider[] _colliders = new Collider[20];
    private static List<LagCompensatedHit> _hits = new();
    [SerializeField] LayerMask LayerMask;
    public override void FixedUpdateNetwork()
    {
        var hitboxes = Runner.LagCompensation.RaycastAll(new Vector3(transform.position.x, 0, transform.position.z), transform.forward, 30f, player: Object.InputAuthority, _hits, LayerMask, clearHits: true);
        for (int i = 0; i < hitboxes; i++)
        {
            // Check for Collectable component on collider game object or any parent.
            var collectable = _hits[i].Hitbox.GetComponentInParent<Collectable>();
            Debug.Log($"{Time.time} {this.name} has collected {collectable.name}");


            if (collectable != null)
            {
                break;
            }
        }
        int collisions = Runner.GetPhysicsScene().OverlapSphere(new Vector3(transform.position.x,0,transform.position.z), 1f, _colliders, LayerMask, QueryTriggerInteraction.Collide);
        for (int i = 0; i < collisions; i++)
        {
            // Check for Collectable component on collider game object or any parent.
            var collectable = _colliders[i].GetComponentInParent<Collectable>();
            if (collectable != null)
            {
                collectable.TryInteracting(this);
                Debug.Log($"{Time.time} {this.name} has collected {collectable.name}");
                // Pickup was successful, activating timer.
                //_activationTimer = TickTimer.CreateFromSeconds(Runner, Cooldown);
                break;
            }
        }
    }
    public override void Render()
    {
        foreach (var change in _changes.DetectChanges(this, out var previousBuffer,out var currentBuffer))
        {
            switch (change)
            {
                case nameof(carriedPocketLoot):
                    var reader = GetPropertyReader<float>(nameof(carriedPocketLoot));
                    var (previous, current) = reader.Read(previousBuffer, currentBuffer);
                    ;break;
                default:
                    break;
            }
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
    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent<Deposit>(out Deposit _deposit);
        if (_deposit != null)
        {
            if (totalPlayerGold <= 0) return;
            carriedPocketLoot = 0;
            carriedRelics = 0;
            _deposit.UpdateGlobalGold_RPC(totalPlayerGold);
            totalPlayerGold = 0;
        }
        //return;
        //other.TryGetComponent<Collectable>(out Collectable _collectable);
        //if (_collectable != null)
        //{
        //    Debug.LogError($"is colliding with this {_collectable.name}");
        //    _collectable.TryInteracting(this);
        //}
    }
    /// <summary>
    /// Checks if the player has enough space for more gold, then picks it up
    /// </summary>
    /// <param name="pCoin"></param> reference to the coin script
    /// <param name="amountToIncrease"></param> increasing amount
    public void CollectCoins(Collectable pCoin, float amountToIncrease)
    {
        if (carriedPocketLoot > (pocketCapacity-amountToIncrease)) return;
        
        totalPlayerGold += amountToIncrease;
        carriedPocketLoot += amountToIncrease;
        Debug.LogWarning($"COLLECTING COINS");
        carriedPocketLoot = Mathf.Clamp(carriedPocketLoot, 0f, pocketCapacity);
        pCoin.DeleteObject();
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
            totalPlayerGold += pAmountToIncrease;
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

   
    private void Pickup()
    {
        GameObject _cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _cube.AddComponent<NetworkObject>();
        
        Runner.Spawn(_cube, this.gameObject.transform.position + (Vector3.forward * 3f), Quaternion.identity);
        if (treasure == null) return;
        treasure.transform.SetPositionAndRotation(carryPoint.position, carryPoint.rotation);
    }
}
