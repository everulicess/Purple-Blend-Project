using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

public class testCharacter : NetworkBehaviour
{
    static testCharacter test_Character;
    [SerializeField] TextMeshProUGUI displayText;
    NetworkString<_32> chosenCharacter;
    string msg;
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            chosenCharacter = PlayerPrefs.GetString("Character");
            test_Character = this;
            msg = $"My Selected Character is {PlayerPrefs.GetString("Character")}";
            displayText.gameObject.SetActive(HasInputAuthority);
            Debug.LogError($"{msg}\n {chosenCharacter}");
            displayText.text = msg;
            RPC_SendMessage(PlayerPrefs.GetString("Character"));
        }
    }
    public override void FixedUpdateNetwork()
    {
        if (!HasInputAuthority)
        {
            displayText.text = " ";
            displayText.gameObject.SetActive(!HasInputAuthority);
        }
        else
        {
            //if(Input.GetKeyDown(KeyCode.Space))
        }
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SendMessage(string message, RpcInfo info = default)
    {
        RPC_RelayMessage(message, info.Source);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_RelayMessage(string message, PlayerRef messageSource)
    {
        string debugMsg = " ";
        if (messageSource == Runner.LocalPlayer)
        {
            debugMsg += $"You said: {message}\n";
        }
        else
        {
            debugMsg += $"Some other player said: {message}\n";
        }
        Debug.LogError(debugMsg);
    }
}
