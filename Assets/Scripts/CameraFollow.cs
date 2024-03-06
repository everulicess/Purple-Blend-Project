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


    [SerializeField] private float camHeight;
    [SerializeField] private float camDistance;
    [SerializeField] private float camXRotationt;
    [SerializeField] private float camYRotation;
    private Vector3 camPosition;
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
        if (target != null)
            camPosition = new Vector3(target.position.x, camHeight, camDistance);
            transform.SetPositionAndRotation(camPosition, Quaternion.Euler(camXRotationt,camYRotation,0));
    }
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
