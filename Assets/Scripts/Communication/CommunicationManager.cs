using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunicationManager : MonoBehaviour
{
    [SerializeField]private Camera cam;
    [SerializeField]private GameObject ping1;
    [SerializeField] private AudioClip ping1Audio;

   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlacePing(ping1, ping1Audio);
        }
    }
    public void PlacePing(GameObject _pingVisual, AudioClip _pingSound)
    {
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
}
