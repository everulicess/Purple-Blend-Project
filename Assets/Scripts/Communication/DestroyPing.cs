using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPing : MonoBehaviour
{
    [SerializeField] float destroyingTime;
    // Update is called once per frame
    void Update()
    {
        destroyingTime -= Time.deltaTime;
        if (destroyingTime<0)
        {
            Destroy(this.gameObject);
        }
    }
}
