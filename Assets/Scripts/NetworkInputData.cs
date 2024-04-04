using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

enum MyButtons
{
    PingsButton,
    InteractButton,
    LeftClick,
    RightClick
}
public struct NetworkInputData : INetworkInput
{
    public Vector3 direction;

    public Vector3 PingPosition;
    public NetworkButtons buttons;
}
