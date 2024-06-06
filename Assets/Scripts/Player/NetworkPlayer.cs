using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using UnityEngine.Events;
using Fusion;
using System;

public class NetworkPlayer : NetworkBehaviour
{
    public static NetworkPlayer Local { get; set; }

    [Header("Class Selection Canvas")]
    [SerializeField] GameObject ClassSelecionCanvas;

    [Header("Classes Data")]
    [SerializeField] CharacterData[] ClassesData;
    [HideInInspector] public CharacterData m_CharacterData { get; set; }
    Animator m_Animator;
    [Networked] private Characters m_Character { get; set; }
    [Networked] bool isInitialized { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        //Subscribe to an event when character selection is done
        ClassSelection.OnCharacterSet += SetCharacterData;
        SelectionMenuHandler();
    }
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;
        }
        else
        {
            ClassSelecionCanvas.SetActive(false);
        }
        m_Animator = GetComponent<Animator>();

        if (isInitialized)
        {
            RPC_EnableModel(m_Character);
            m_Animator.avatar = m_CharacterData.CharacterAvatar;
            m_Animator.runtimeAnimatorController = m_CharacterData.AnimationController;
        }
    }
    private void SelectionMenuHandler()
    {
        if (NetworkPlayer.Local)
        {
            ClassSelecionCanvas.SetActive(HasInputAuthority);
        }
        else
        {
            ClassSelecionCanvas.SetActive(false);
        }
    }
    public void SetCharacterData(Characters nameOfSpawningModel)
    {
        Debug.LogError($"Has Input Authority {HasInputAuthority}");
        if (!Object.HasInputAuthority)
            return;
        if (!NetworkPlayer.Local)
            return;
        foreach (CharacterData data in ClassesData)
        {
            if (data.Name == nameOfSpawningModel)
            {
                m_Character = data.Name;
            }
        }
        ClassSelection.OnCharacterSet -= SetCharacterData;
        InitializeCharacterData();
    }
    private void InitializeCharacterData()
    {
        if (!NetworkPlayer.Local)
            return;
        if (!HasInputAuthority)
            return;
        RPC_EnableModel(m_Character);
        
        isInitialized = true;
        
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_EnableModel(Characters myCharacter, RpcInfo info = default)
    {
        RPC_RelayenableModel(myCharacter, info.Source);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_RelayenableModel(Characters myCharacter, PlayerRef messageSource)
    {
        string debugMsg = " ";
        if (messageSource == Runner.LocalPlayer)
        {
            RPCForeachLoop(myCharacter);
            debugMsg += $"You said: {m_CharacterData.Model}\n with character {m_Character}\n";
        }
        else
        {
            RPCForeachLoop(myCharacter);
            debugMsg += $"Some other player said: {m_CharacterData.Model}\nwith character {m_Character}\n";
        }
        m_Animator.avatar = m_CharacterData.CharacterAvatar;
        m_Animator.runtimeAnimatorController = m_CharacterData.AnimationController;
        Debug.LogError(debugMsg);
    }

    private void RPCForeachLoop(Characters myCharacter)
    {
        foreach (CharacterData item in ClassesData)
        {
            if (myCharacter == item.Name)
            {
                m_CharacterData = item;
            }
            else
            {
                Runner.Despawn(item.Model);
            }
        }
    }
}
[Serializable]
public struct CharacterData
{
    [Header("Player Data")]
    public Characters Name;
    public CharacterStatsScrObj StatsData;

    [Header("Player Model")]
    public NetworkObject Model;

    [Header("Animations Data")]
    public AnimatorController AnimationController;
    public Avatar CharacterAvatar;

}
