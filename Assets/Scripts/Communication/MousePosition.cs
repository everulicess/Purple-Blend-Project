using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePosition : MonoBehaviour
{
    public static RectTransform _rect;
    //[SerializeField] private CursorLockMode _cursorLockState = CursorLockMode.None;
    //private Camera cam;
    public static Vector3 InWorldRayPosition;
    //[SerializeField] GameObject debugObject;
    void Start()
    {
        if (Input.mousePresent)
        {
            _rect = GetComponent<RectTransform>();

            if (!Application.isEditor)
                Cursor.visible = false;
            //Cursor.lockState = _cursorLockState;
        }
        else
            gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (!Input.GetKey(KeyCode.V))
        {
            _rect.position = Input.mousePosition;
        }
        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    Vector3 offsetTry = FindObjectOfType<LocalCamera>().GetComponentInChildren<Camera>().ScreenToWorldPoint(_rect.position);
        //    InWorldRayPosition = offsetTry;

        //}

        if (Input.GetKeyDown(KeyCode.V) || Input.GetMouseButtonDown(0))
        {
            Ray ray = FindObjectOfType<LocalCamera>().GetComponentInChildren<Camera>().ScreenPointToRay(_rect.position);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject.layer != 8) return;

                Vector3 offset = new(hit.point.x, hit.point.y + 0.1f, hit.point.z);
                InWorldRayPosition = offset;
            }
        }
    }
}
