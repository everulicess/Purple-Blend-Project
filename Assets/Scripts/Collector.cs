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
    private float carriedPocketLoot;

    //UI element (doesnt work)
    public Image pocketBar;

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Loot")
        {
            Debug.Log("Player has detected loot");
            if (carriedPocketLoot < pocketCapacity)
            {
                carriedPocketLoot += 25;
                carriedPocketLoot = Mathf.Clamp(carriedPocketLoot, 0, pocketCapacity);
                Debug.Log($"the player is carrying {carriedPocketLoot} gold");
                Destroy(col.transform.gameObject);

                //Set player UI bar to filled %
                pocketBar.fillAmount = carriedPocketLoot / pocketCapacity * 100f;
            }
        }
    }
}
