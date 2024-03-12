using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    GameObject target;
    public Transform Player;

    private void Start()
    {
        target = GameObject.Find("Camera");
    }
    void Update()
    {
        Vector3 targetPosition = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        transform.LookAt(targetPosition);
        transform.position = new Vector3(Player.transform.position.x, Player.position.y + 1.5f, Player.transform.position.z);
    }
}
