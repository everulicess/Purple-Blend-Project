using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Singleton
    {
        get => singleton;
        set
        {
            if (value == null)
                singleton = null;
            else if (singleton == null)
                singleton = value;
            else if (singleton != value)
            {
                Destroy(value);
                Debug.LogError($"Only one instance of {nameof(CameraFollow)}!");
            }
        }
    }
    private static CameraFollow singleton;

    private Transform target;


    [Header("Zoom")]
    [SerializeField] private float distance = 5f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float minDistance = 1f;
    //[SerializeField] private Vector3 offset;
    [Header("Speeds")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float scrollSensitivity = 1;

    //[SerializeField] private float Zoom;
    //[SerializeField] private float camHeight;
    //[SerializeField] private float camDistance;
    [Header("Rotation")]
    [SerializeField] private float camXRotationt;
    [SerializeField] private float camYRotation;
    //private Vector3 camPosition;
    private void Awake()
    {
        Singleton = this;
    }
    private void OnDestroy()
    {
        if (Singleton == this)
            Singleton = null;
    }
    private void LateUpdate()
    {
        //Vector3 zoomIn = new Vector3(0,camHeight);

        if (target == null) return;

        float num = Input.GetAxis("Mouse ScrollWheel");
        distance -= num * scrollSensitivity;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        Vector3 pos = target.position;
        pos -= transform.forward * distance;

        transform.position = Vector3.Lerp(transform.position, pos, smoothSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(camXRotationt, camYRotation, 0f);
        this.gameObject.GetComponent<Camera>().orthographicSize = distance;

        //Zoom = Mathf.Clamp(Zoom, minDistance, maxDistance);
        //this.gameObject.GetComponent<Camera>().orthographicSize = Mathf.Clamp(this.gameObject.GetComponent<Camera>().orthographicSize, 0.4f, 8f);

        //camPosition = new Vector3(target.position.x, camHeight, target.position.z + camDistance);
        //transform.SetPositionAndRotation(camPosition, Quaternion.Euler(camXRotationt, camYRotation, 0));
    }
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
