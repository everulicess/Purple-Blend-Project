using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class Collector : NetworkBehaviour
{
    //Storage variables
    //Coins
    [Range(1f, 500f)]
    [SerializeField] float pocketCapacity;
    float carriedPocketLoot;

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
    //UI element (doesn't work)
    public Image pocketBar;
    bool isInteracting;
    private void Update()
    {
        if (isInteracting)
        {
            Debug.LogWarning("Player Is Interacting");
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
        other.TryGetComponent<Collectable>(out Collectable _collectable);
        if (_collectable != null)
        {
            _collectable.TryInteracting(this);
        }

        other.TryGetComponent<Deposit>(out Deposit _deposit);
        if (_deposit != null)
        {
            if (totalPlayerGold <= 0) return;
            carriedPocketLoot = 0;
            carriedRelics = 0;
            _deposit.UpdateGlobalGold_RPC(totalPlayerGold);
            totalPlayerGold = 0;
        }
        
    }
    /// <summary>
    /// Checks if the player has enough space for more gold, then picks it up
    /// </summary>
    /// <param name="pCoin"></param> reference to the coin script
    /// <param name="amountToIncrease"></param> increasing amount
    public void CollectCoins(Collectable pCoin, float amountToIncrease)
    {
        if (carriedPocketLoot > (pocketCapacity-amountToIncrease)) return;
        //{
        //    Debug.Log("Your pockets are full!");
        //    return;
        //}
        totalPlayerGold += amountToIncrease;
        carriedPocketLoot += amountToIncrease;
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

    private void Treasure(Collider other)
    {
        //treasure = carryingTreasure ? other.gameObject : null;
        Debug.Log("Player can pick up a treasure!");
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
