using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fusion;

public enum Pings
{
    Ping1,
    Ping2,
    Ping3,
    Ping4,
    Ping5
}
[Serializable]
public struct PingsVisualsLibrary
{
    public GameObject Ping1;
    public GameObject Ping2;
    public GameObject Ping3;
    public GameObject Ping4;
    public GameObject Ping5;
}
[Serializable]
public struct PingsSoundsLibrary
{
    public AudioClip Audio1;
    public AudioClip Audio2;
    public AudioClip Audio3;
    public AudioClip Audio4;
    public AudioClip Audio5;
}
public class CommunicationManager : MonoBehaviour
{
    [SerializeField]PingsSoundsLibrary pingSoundLibrary;
    [SerializeField]PingsVisualsLibrary pingVisualLibrary;

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
    public void PlacePing(/*GameObject _pingVisual,*/ /*AudioClip _pingSound,*/ Pings _ping)
    {
        //get vsound and visuals for each ping
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
        audioDictionary.Add(Pings.Ping1, pingSoundLibrary.Audio1);
        audioDictionary.Add(Pings.Ping2, pingSoundLibrary.Audio2);

        //Visual
        visualsDictionary.Add(Pings.Ping1, pingVisualLibrary.Ping1);
        visualsDictionary.Add(Pings.Ping2, pingVisualLibrary.Ping2);
    }
}


