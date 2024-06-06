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
    [HideInInspector] public CharacterData m_CharacterData;
    Animator m_Animator;

    GameObject m_Model { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        //Subscribe to an event when character selection is done
        ClassSelection.OnCharacterSet += SpawnTheModel;
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
        foreach (CharacterData item in ClassesData)
        {
            item.Model.SetActive(false);
        }
        m_Animator = GetComponent<Animator>();
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
    public void SpawnTheModel(Characters nameOfSpawningModel)
    {
        //RPC_SpawnModel(nameOfSpawningModel);
        
        foreach (CharacterData data in ClassesData)
        {
            if (data.Name == nameOfSpawningModel)
            {
                m_Model = data.Model;
                m_CharacterData = data;
            }
        }
        ClassSelection.OnCharacterSet -= SpawnTheModel;
        InitializeCharacterData();
    }
    private void InitializeCharacterData()
    {
        if (!NetworkPlayer.Local)
            return;
        if (!HasInputAuthority)
            return;
        //model activation
        RPC_EnableModel();
        m_Animator.avatar = m_CharacterData.CharacterAvatar;
        m_Animator.runtimeAnimatorController = m_CharacterData.AnimationController;
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_EnableModel( RpcInfo info = default)
    {
        RPC_RelayenableModel( info.Source);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All/*, HostMode = RpcHostMode.SourceIsServer*/)]
    public void RPC_RelayenableModel(PlayerRef messageSource)
    {
        string debugMsg = " ";
        foreach (CharacterData data in ClassesData)
        {
            if (data.Model == m_Model)
            {
                m_Model.SetActive(true);
                Debug.LogError(m_Model);
            }
        }

        //if (messageSource == Runner.LocalPlayer)
        //{
        //    m_CharacterData.Model.SetActive(true);

        //    debugMsg += $"You said: {m_CharacterData.Model}\n";
        //}
        //else
        //{

        //    debugMsg += $"Some other player said: {m_CharacterData.Model}\n";
        //}
        Debug.LogError(debugMsg);
    }
}
[Serializable]
public struct CharacterData
{
    [Header("Player Data")]
    public Characters Name;
    public CharacterStatsScrObj StatsData;

    [Header("Player Model")]
    public GameObject Model;

    [Header("Animations Data")]
    public AnimatorController AnimationController;
    public Avatar CharacterAvatar;
}
