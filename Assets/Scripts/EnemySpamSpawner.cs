using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpamSpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    private float charCounter = 9f;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            charCounter++;
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            Debug.Log("There are " + charCounter + " characters in the scene.");
        }
    }
}
