using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collector : MonoBehaviour
{
    //Variables for all the loot stuff
    [Range(1f, 500f)]
    public float pocketCapacity;
    [Range(1f, 4f)]
    public float relicCapacity;
    public float carriedRelics;
    private float carriedPocketLoot;
    private bool carryingTreasure = false;
    [SerializeField] private Transform carryPoint;
    Transform treasurePosition;

    //UI element (doesnt work)
    public Image pocketBar;

    //Collecting gold
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Loot")
        {
            Debug.Log("Player has detected loot");

            // if the player is not full
            if (carriedPocketLoot < pocketCapacity)
            {
                carriedPocketLoot += 25;
                carriedPocketLoot = Mathf.Clamp(carriedPocketLoot, 0, pocketCapacity);
                Debug.Log($"the player is carrying {carriedPocketLoot} gold");
                Destroy(col.transform.gameObject);

                //Set player UI bar to filled %
                pocketBar.fillAmount = carriedPocketLoot / pocketCapacity * 100f;
            }
            else
            {
                Debug.Log("Your pockets are full!");
            }
        }
        if (col.gameObject.tag == "Relic")
        {
            Debug.Log("Player can pick up a relic");
            if (Input.GetKey(KeyCode.E))
            {
                //If the player has a relic slot left
                if (carriedRelics < relicCapacity)
                {
                    carriedRelics++;
                    Destroy(col.transform.gameObject);
                }
                else
                {
                    Debug.Log("Your relic slots are full!");
                }
            }

        }
        // collecting treasure
        if (col.gameObject.tag == "Treasure")
        {
            //if (carryingTreasure == false)
            if (carryingTreasure) return;
            //{
            //Debug.Log("Player can pick up a treasure!");
            if (Input.GetKey(KeyCode.E))
            {
                carryingTreasure = true;
                treasurePosition = col.transform;
                Debug.Log("Player picked up treasure!");
            }
            //}
        }

        //This is for depositing your pockets into the mule or spawn area to score.
        if (col.gameObject.tag == "Deposit")
        {
            //if (Input.GetKey(KeyCode.E))
        }
    }

    //Collecting Relics
        private void OnTriggerStay(Collider col)
        {
        //    if (col.gameObject.tag == "Relic")
        //    {
        //        Debug.Log("Player can pick up a relic");
        //        if (Input.GetKey(KeyCode.E))
        //        {
        //            //If the player has a relic slot left
        //            if (carriedRelics < relicCapacity)
        //            {
        //                carriedRelics++;
        //                Destroy(col.transform.gameObject);
        //            }
        //            else
        //            {
        //                Debug.Log("Your relic slots are full!");
        //            }
        //        }

        //    }
        //// collecting treasure
        //    if (col.gameObject.tag == "Treasure")
        //    {
        //        //if (carryingTreasure == false)
        //        if (carryingTreasure) return;
        //        //{
        //        //Debug.Log("Player can pick up a treasure!");
        //        if (Input.GetKey(KeyCode.E))
        //        {
        //            carryingTreasure = true;
        //            treasurePosition = col.transform;
        //            Debug.Log("Player picked up treasure!");
        //        }
        //        //}
        //    }

        //    //This is for depositing your pockets into the mule or spawn area to score.
        //    if (col.gameObject.tag == "Deposit")
        //        {
        //        //if (Input.GetKey(KeyCode.E))
        //        }

    }
    private void Update()
    {
        //If the player is carrying a treasure
        if (carryingTreasure)
        {
            //follow the player carrypoint
            Pickup();

            //release the carried treasure
            if (Input.GetKeyDown(KeyCode.E) && carryingTreasure == true)
            {
                DropItem();
            }
        }
    }

    private void DropItem()
    {
        treasurePosition.position = treasurePosition.position;
        carryingTreasure = false;
        Debug.Log("Player dropped a treasure!");
    }

    private void Pickup()
    {
        if (treasurePosition == null) return;
        treasurePosition.position = carryPoint.position;
        treasurePosition.rotation = carryPoint.rotation;
    }

    private void deposit()
    {
        //This is where it checks who it can deposit to, and then transfers all carried goods over to that deposit.

        //check for mule or main deposit

        //mule
        //check for if space left and how much

        //main
        //deposit all goods and add to score
    }
}
