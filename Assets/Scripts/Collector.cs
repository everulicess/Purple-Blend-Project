using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collector : MonoBehaviour
{
    //Variables for player pockets
    [Range(1f, 500f)]
    public float pocketCapacity;
    [Range(1f, 4f)]
    public float relicCapacity;
    public float carriedRelics;
    private float carriedPocketLoot;

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
    }

    //Collecting Relics
        private void OnTriggerStay(Collider col)
        {
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

            if (col.gameObject.tag == "Deposit")
            {
            //if (Input.GetKey(KeyCode.E))
            }

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
