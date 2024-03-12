using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fusion;

public enum Pings
{
    None,
    MissingPing,
    LocationPing
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

    [SerializeField] private Camera cam;

    [SerializeField] private GameObject pingMenuUIObject;
    private Vector3 pingMenuPosition;
    [SerializeField] PingElement[] ringElements;
    
    Pings currentPing;
    private void Awake()
    {
        InitializeDictionaries();

        pingMenuUIObject.SetActive(false);

        currentPing = Pings.MissingPing;
        //ringElements = FindObjectsOfType<PingElement>();

        foreach (PingElement element in ringElements)
        {
            element.SelectedPingEvent += PlacePing;
        }

    }
    void Update()
    {
        //need logic to change current ping to specific ping based on the ring menu
        
        if (Input.GetKeyDown(KeyCode.M))
        {
            currentPing = Pings.LocationPing;
            PlacePing(currentPing);
        } if (Input.GetKeyDown(KeyCode.N))
        {
            currentPing = Pings.MissingPing;
            PlacePing(currentPing);
        }
        if (pingMenuUIObject == null) return;

        if (Input.GetKey(KeyCode.V))
        {
            
            pingMenuUIObject.SetActive(true);
            pingMenuUIObject.transform.position = pingMenuPosition;
            //Cursor.visible = false;
        }
        else
        {
            //CloseMenu();
            Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
            pingMenuPosition = Input.mousePosition;
            pingMenuUIObject.SetActive(false);
        }
    }
    public void CloseMenu()
    {
        
        PlacePing(currentPing);
    }
    void PlacePing(Pings _ping)
    {
        if (!Input.GetKeyUp(KeyCode.V)) return;
        if (_ping == Pings.None) return;
        //get sound and visuals for each ping
        visualsDictionary.TryGetValue(_ping, out GameObject _pingVisual);
        audioDictionary.TryGetValue(_ping, out AudioClip _pingSound);

        Ray ray = cam.ScreenPointToRay(pingMenuPosition);
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
        audioDictionary.Add(Pings.MissingPing, communicationLibrary.Ping1.Sound);
        audioDictionary.Add(Pings.LocationPing, communicationLibrary.Ping2.Sound);

        //Visual
        visualsDictionary.Add(Pings.MissingPing, communicationLibrary.Ping1.Prefab);
        visualsDictionary.Add(Pings.LocationPing, communicationLibrary.Ping2.Prefab);
    }
}


