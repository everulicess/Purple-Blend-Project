using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

enum MyButtons
{
    PingsButton,
    PingsButtonReleased,
    PartyButton,
    InteractButton,
    AttackButton,
    SpecialButton,
    DodgeButton,
    TestingButtonQ,
    MenuButton
}
public struct NetworkInputData : INetworkInput
{
    
    //Movement and rotation
    public Vector3 movementInput;
    public Vector3 pointToLookAt;
    //buttons
    public NetworkButtons buttons;
}
