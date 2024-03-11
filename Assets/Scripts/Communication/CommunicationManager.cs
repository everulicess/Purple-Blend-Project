using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fusion;

public enum Pings
{
    Ping1,
    Ping2
}
[Serializable]
public struct PingInfo
{
    [Tooltip("3D visual for the ping")] public GameObject Prefab;
    [Tooltip("It will be played when the ping is placed")] public AudioClip Sound;
}
[Serializable]
public struct CommunicationLibrary
{
    [Header("Pings")]
    public PingInfo Ping1;
    public PingInfo Ping2;
}
public class CommunicationManager : MonoBehaviour
{
    [SerializeField]CommunicationLibrary communicationLibrary;

    public static Dictionary<Pings, AudioClip> audioDictionary = new();
    public static Dictionary<Pings, GameObject> visualsDictionary = new();

    [SerializeField]private Camera cam;

    [SerializeField] private GameObject pingMenuUIObject;
    private Vector3 pingMenuPosition;
    Pings currentPing;
    private void Awake()
    {
        InitializeDictionaries();

        pingMenuUIObject.SetActive(false);

        currentPing = Pings.Ping1;
    }
    void Update()
    {
        //need logic to change current ping to specific ping based on the ring menu
        
        if (Input.GetKeyDown(KeyCode.M))
        {
            currentPing = Pings.Ping2;
            PlacePing(currentPing);
        } if (Input.GetKeyDown(KeyCode.N))
        {
            currentPing = Pings.Ping1;
            PlacePing(currentPing);
        }
        if (pingMenuUIObject == null) return;

        if (Input.GetKey(KeyCode.V))
        {
            pingMenuUIObject.SetActive(true);
            pingMenuUIObject.transform.position = pingMenuPosition;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            pingMenuPosition = Input.mousePosition;
            pingMenuUIObject.SetActive(false);
        }
    }
    public void PlacePing(Pings _ping)
    {
        //get sound and visuals for each ping
        visualsDictionary.TryGetValue(_ping, out GameObject _pingVisual);
        audioDictionary.TryGetValue(_ping, out AudioClip _pingSound);

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject.layer==8)
            {
                Vector3 offset = new Vector3(hit.point.x, hit.point.y + 0.1f, hit.point.z);
                Instantiate(_pingVisual, offset, Quaternion.identity);
                AudioSource.PlayClipAtPoint(_pingSound, offset);

                Debug.Log($"{_pingVisual.name} has been placed and {_pingSound.name} is ebing played");
            }
        }
    }

    private void InitializeDictionaries()
    {
        //Sound
        audioDictionary.Add(Pings.Ping1, communicationLibrary.Ping1.Sound);
        audioDictionary.Add(Pings.Ping2, communicationLibrary.Ping2.Sound);

        //Visual
        visualsDictionary.Add(Pings.Ping1, communicationLibrary.Ping1.Prefab);
        visualsDictionary.Add(Pings.Ping2, communicationLibrary.Ping2.Prefab);
    }
}


