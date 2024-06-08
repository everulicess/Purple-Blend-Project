using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerUIHandler : NetworkBehaviour
{
    [SerializeField] GameObject HUDUIPanel;
    [SerializeField] GameObject InGameUIPanel;
    public override void Spawned()
    {
        if (Player.Local)
        {
            HUDUIPanel.SetActive(HasInputAuthority);
            InGameUIPanel.SetActive(!HasInputAuthority);
        }
        else
        {
            HUDUIPanel.SetActive(false);
            InGameUIPanel.SetActive(true);
        }
    }

}
