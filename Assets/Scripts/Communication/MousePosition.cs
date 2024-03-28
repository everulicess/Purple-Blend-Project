using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePosition : MonoBehaviour
{
    public static RectTransform _rect;
    [SerializeField] private CursorLockMode _cursorLockState = CursorLockMode.None;

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
    }
}
