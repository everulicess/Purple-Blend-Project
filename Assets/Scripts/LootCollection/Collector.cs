using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class Collector : NetworkBehaviour
{
    //UI
    [Header("User Interface")]
    [Header("Coins Pocket")]
    [SerializeField] GameObject pocketInGameUI;
    [SerializeField] GameObject pocketHUDUI;
    [SerializeField] Image[] currentPocketBar;
    [Header("Relic Spots")]
    [SerializeField] GameObject relicInGameUI;
    [SerializeField] GameObject relictHUDUI;
    Image[] relicSpotsHUD;
    Image[] relicSpotsInGame;
    [Header("Interact")]
    [SerializeField] GameObject InteractUI;

    //Storage variables
    ChangeDetector _changes;

    //Coins
    [Range(1f, 500f)]
    [SerializeField] float pocketCapacity;
    [Networked] float CarriedPocketLoot { get; set; }
    int CollectedCoins;
    readonly float coinsValue = 50f;

    //Relics
    [Range(1, 4)]
    [SerializeField] int relicCapacity;
    [Networked] int carriedRelics { get; set; }
    readonly float relicValue = 100f;
    [SerializeField] Sprite collectedRelic;
    [SerializeField] Sprite emptyRelic;

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
        //InteractUI.SetActive(false);
        relicSpotsHUD = relictHUDUI.GetComponentsInChildren<Image>();
        relicSpotsInGame = relicInGameUI.GetComponentsInChildren<Image>();
        RelicsUIUpdate();
        OnCarriedPocketChange();
        if (Player.Local)
        {
            CarriedPocketLoot = 0;
        }
    }
    private void Update()
    {
        if (Player.Local)
        {
            //Coin bar
            pocketHUDUI.SetActive(HasInputAuthority);
            pocketInGameUI.SetActive(!HasInputAuthority);

            //Relic Spots
            relictHUDUI.SetActive(HasInputAuthority);
            relicInGameUI.SetActive(!HasInputAuthority);

        }
        else
        {
            pocketHUDUI.SetActive(false);
            pocketInGameUI.SetActive(true);

            relictHUDUI.SetActive(false);
            relicInGameUI.SetActive(true);
        }

        pocketInGameUI.transform.rotation = Quaternion.Euler(30, 45, 0);
        relicInGameUI.transform.rotation = Quaternion.Euler(30, 45, 0);
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

            _colliders[i].TryGetComponent(out Deposit _deposit);
            var deposit = _deposit;
            if (deposit != null)
            {
                Deposit(deposit);
            }
        }
    }
    public override void Render()
    {
        InteractUI.transform.rotation = Quaternion.Euler(30f, 45f, 0);

        foreach (var change in _changes.DetectChanges(this, out var previousBuffer,out var currentBuffer))
        {
            switch (change)
            {
                case nameof(CarriedPocketLoot):
                    var reader = GetPropertyReader<float>(nameof(CarriedPocketLoot));
                    var (previous, current) = reader.Read(previousBuffer, currentBuffer);
                    OnCarriedPocketChange()
                    ;break;
                case nameof(carriedRelics):
                    var intReader = GetPropertyReader<int>(nameof(carriedRelics));
                    var (previousInt, currentInt) = intReader.Read(previousBuffer, currentBuffer);
                    OnCarriedRelicsChange(previousInt,currentInt)
                    ;break;
                default:
                    break;
            }
        }
    }
    private void OnCarriedRelicsChange(int previous, int current)
    {
        if (previous < current)
            RelicsUIUpdate();

        if (carriedRelics==0)
        {
            ResetRelicsUI();
        }
    }

    private void ResetRelicsUI()
    {
        foreach (Image image in relicSpotsHUD)
            image.sprite = emptyRelic;

        foreach (Image image in relicSpotsInGame)
            image.sprite = emptyRelic;
    }

    private void RelicsUIUpdate()
    {
        for (int image = 0; image < carriedRelics; image++)
        {
            relicSpotsHUD[image].sprite = collectedRelic;
            relicSpotsInGame[image].sprite = collectedRelic;
        }
    }

    private void OnCarriedPocketChange()
    {

        foreach (Image image in currentPocketBar)
        {
            image.fillAmount = CarriedPocketLoot / pocketCapacity;
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
    bool isCoin;
    /// <summary>
    /// Checks if the player has enough space for more gold, then picks it up
    /// </summary>
    /// <param name="pCoin"></param> reference to the coin script
    /// <param name="amountToIncrease"></param> increasing amount
    public void CollectCoins(Collectable pCoin, float amountToIncrease)
    {
        isCoin = true;
        if (CarriedPocketLoot > (pocketCapacity-amountToIncrease)) return;
        pCoin.DeleteObject();
        CollectedCoins++;
        CarriedPocketLoot = CollectedCoins * coinsValue;
        CarriedPocketLoot = Mathf.Clamp(CarriedPocketLoot, 0f, pocketCapacity);
        SoundManager.RPC_PlaySound(SoundName.CoinsPickup, this.transform.position);
    }
    /// <summary>
    /// Checks if the player is trying to interact and if there is anough space for the relic
    /// </summary>
    /// <param name="pRelic"></param> refrence to the relic, to destroy it if it's collected
    public void CollectRelic(Collectable pRelic, float pAmountToIncrease)
    {
        isCoin = false;
        InteractUI.transform.position = new(pRelic.transform.position.x, pRelic.transform.position.y + 1f, pRelic.transform.position.z);
        if (isInteracting)
        {
            //If the player has a relic slot left
            if (carriedRelics >= relicCapacity)
                return;
            carriedRelics++;
            SoundManager.RPC_PlaySound(SoundName.RelicPickup, this.transform.position);
            pRelic.DeleteObject();
            InteractUI.SetActive(false);
        }
    }
    public bool CanPickUp()
    {
        return !carryingTreasure;
    }
    public void CollectTreasure(Collectable pTreasure, Rigidbody pRigidBody, BoxCollider pCollider)
    {
        isCoin = false;

        InteractUI.transform.position = new(pTreasure.transform.position.x, pTreasure.transform.position.y + 1f, pTreasure.transform.position.z);
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9&&!isCoin) 
            InteractUI.SetActive(HasInputAuthority);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer==9)
            InteractUI.SetActive(false);
    }
    private void Deposit(Deposit _deposit)
    {
        isCoin = false;
        totalPlayerGold = (CollectedCoins * coinsValue) + (carriedRelics * relicValue);
        InteractUI.transform.position = new(_deposit.transform.position.x, _deposit.transform.position.y + 2f, _deposit.transform.position.z);
        if (isInteracting)
        {
            if (totalPlayerGold <= 0) return;
            CarriedPocketLoot = 0;
            carriedRelics = 0;
            CollectedCoins = 0;
            _deposit.UpdateGlobalGold(totalPlayerGold);
            SoundManager.RPC_PlaySound(SoundName.LootDelivered, this.transform.position);
            totalPlayerGold = 0;
            foreach (Image image in relicSpotsHUD)
            {
                image.color = Color.white;
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
