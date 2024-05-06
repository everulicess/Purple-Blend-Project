using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CharacterInputHandler : SimulationBehaviour
{
    Vector3 moveInputVector = Vector3.zero;
    Vector3 mouseWorldPosition;
    Player m_Player;
    NetworkRunner m_Runner;
    //Buttons
    bool PingButtonPressed = false;
    bool PingButtonReleased = false;
    bool InteractButtonPressed = false;
    bool LeftClickPressed = false;

    //testing button
    bool TestingButtonQPressed = false;

    private void Awake()
    {
        m_Player = GetComponent<Player>();
        m_Runner = FindObjectOfType<NetworkRunner>();
    }
    private void Update()
    {

        //Collect Input
        //Movement input
        moveInputVector = new(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        //Interaction using "E"
        if (Input.GetKeyDown(KeyCode.E))
            InteractButtonPressed = true;
        
        //Open Ping Menu using "V"
        if (Input.GetKey(KeyCode.V))
            PingButtonPressed = true;
        if (Input.GetKeyUp(KeyCode.V))
            PingButtonReleased = true;

        //Attack left mouse button
        if (Input.GetMouseButtonDown(0))
            LeftClickPressed = true;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.V))
        {
            SetInWorldMousePosition();
        }

    }
    //returns collected input to the server
    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new();

        //Movement data
        networkInputData.movementInput = moveInputVector;

        //Rotate towards data
        networkInputData.pointToLookAt = mouseWorldPosition;

        //Buttons Data
        //Interact "E"
        networkInputData.buttons.Set(MyButtons.InteractButton, InteractButtonPressed);
        
        //PingButton pressing V
        networkInputData.buttons.Set(MyButtons.PingsButton, PingButtonPressed);

        //Ping Button released
        networkInputData.buttons.Set(MyButtons.PingsButtonReleased, PingButtonReleased);

        //Attacking LMB
        //networkInputData.isAttackingPressed = LeftClickPressed;
        networkInputData.buttons.Set(MyButtons.AttackButton, LeftClickPressed);

        networkInputData.buttons.Set(MyButtons.TestingButtonQ, Input.GetKeyDown(KeyCode.Q) || TestingButtonQPressed);

        //Reset variables after reading their values
        InteractButtonPressed = false;
        PingButtonPressed = false;
        LeftClickPressed = false;
        PingButtonReleased = false;
        TestingButtonQPressed = false;
        return networkInputData;
    }
    public void SetInWorldMousePosition()
    {
        if (Player.Local == null) 
            return;
        Camera pCameraForRaycast;
        pCameraForRaycast = Player.PlayerCamera;
        Ray ray = pCameraForRaycast.ScreenPointToRay(Input.mousePosition);

        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray.origin, ray.direction, 100000.0F);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.layer != 8) 
                return;
            if (mouseWorldPosition == new Vector3(hit.point.x, hit.point.y, hit.point.z))
                return;

            mouseWorldPosition = new(hit.point.x, hit.point.y, hit.point.z);
            
            Debug.LogError($"Raycasting, the ray hit {hit.collider.gameObject.name}");
        }
    }
}
