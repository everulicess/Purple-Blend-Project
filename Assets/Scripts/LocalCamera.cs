using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalCamera : MonoBehaviour
{
    private Transform target;

    [Header("Zoom")]
    [SerializeField] private float distance = 5f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float minDistance = 1f;
    [Header("Speeds")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float scrollSensitivity = 1;
    [Header("Rotation")]
    [SerializeField] private float camXRotationt = 30f;
    [SerializeField] private float camYRotation = 45f;
    private void Start()
    {
        this.gameObject.name = $"{this.name} + {target.GetComponentInParent<Player>().gameObject.name}";
    }
    private void LateUpdate()
    {
        if (target == null) return;

        float num = Input.GetAxis("Mouse ScrollWheel");
        distance -= num * scrollSensitivity;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        Vector3 pos = target.position;
        pos -= transform.forward * distance;

        transform.position = Vector3.Lerp(transform.position, pos, smoothSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(camXRotationt, camYRotation, 0f);
        this.gameObject.GetComponentInChildren<Camera>().orthographicSize = distance;

    }
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
