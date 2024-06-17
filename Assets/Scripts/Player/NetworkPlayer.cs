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
    [SerializeField] List<CharacterData> ClassesData = new();
    [HideInInspector] public CharacterData m_CharacterData { get; set; }
    [Networked] private Characters m_Character { get; set; }
    [Networked] bool isInitialized { get; set; } = false;
    NetworkBool initiated;

    #region PLAYER_COMPONENTS_&_SCRIPTS

    [SerializeField] PlayerComponents Components = new();
    private void InitializeComponents(bool _enable)
    {
        //if (!NetworkPlayer.Local)
        //    return;
      
        //if (!HasInputAuthority)
        //    return;
        Debug.LogError($"NetworkBool {initiated} Networked Bool: {isInitialized} character: {m_Character} ");

        //Components.m_NetworkCharacterController = GetComponent<NetworkCharacterController>();
        Components.m_NetworkCharacterController.enabled = _enable;

        //Components.m_CharacterController = GetComponent<CharacterController>();
        Components.m_CharacterController.enabled = _enable;

        //Components.m_Animator = GetComponent<Animator>();
        //Components.m_Animator.enabled = _enable;

        //Components.m_NetworkAnimator = GetComponent<NetworkMecanimAnimator>();
        //Components.m_NetworkAnimator.enabled = _enable;



        //Components.m_Player = GetComponent<Player>();
        Components.m_Player.enabled = _enable;

        //Components.m_Health = GetComponent<Health>();
        Components.m_Health.enabled = _enable;

        //Components.m_CharacterInputHandler = GetComponent<CharacterInputHandler>();
        Components.m_CharacterInputHandler.enabled = _enable;

        //Components.m_CombatController = GetComponent<CombatController>();
        Components.m_CombatController.enabled = _enable;

        //Components.m_PlayerCommunication = GetComponent<PlayerCommunication>();
        Components.m_PlayerCommunication.enabled = _enable;

        //Components.m_Collector = GetComponent<Collector>();
        Components.m_Collector.enabled = _enable;

    }
    #endregion

    ChangeDetector Changes;
    // Start is called before the first frame update
    void Start()
    {
        ClassSelection.OnCharacterSet += SetCharacterData;
        SelectionMenuHandler();
    }
    public override void Spawned()
    {
        Changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
        if (Object.HasInputAuthority)
        {
            Local = this;
        }
        else
        {
            ClassSelecionCanvas.SetActive(false);
        }
        //InitializeComponents(isInitialized);
        Debug.LogError($"character{m_Character} is initilaized: {initiated}");
        if (initiated)
        {
            RPC_EnableModel(m_Character);
            InitializeComponents(initiated);
            if (!NetworkPlayer.Local)
                return;
            Components.m_Animator = m_CharacterData.Model.GetComponent<Animator>();
            Components.m_Animator.avatar = m_CharacterData.CharacterAvatar;
            Components.m_Animator.runtimeAnimatorController = m_CharacterData.AnimationController;
        }
    }
    public override void Render()
    {
        if (initiated)
        {
            InitializeComponents(true);
            Components.m_Player.SetPlayerVariables(m_CharacterData.StatsData, Components.m_Animator);
        }
        else
        {
            InitializeComponents(false);
        }
        

        foreach (var change in Changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(initiated):
                    var boolReader = GetPropertyReader<bool>(nameof(initiated));
                    var (previousBool, currentBool) = boolReader.Read(previousBuffer, currentBuffer);
                    ; break;
            }
        }
        if (!initiated)
            return;
        RPC_EnableModel(m_Character);
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

        if (!NetworkPlayer.Local)
            return;
        if (!Object.HasInputAuthority)
            return;
        if (initiated)
            return;
        ClassSelection.OnCharacterSet -= SetCharacterData;

        foreach (CharacterData data in ClassesData)
        {
            if (data.Name == nameOfSpawningModel)
            {
                m_Character = data.Name;
            }
        }
        InitializeCharacterData();
    }
    private void InitializeCharacterData()
    {
        if (!HasInputAuthority)
            return;
        if (!NetworkPlayer.Local)
            return;
        RPC_EnableModel(m_Character);
        Debug.LogError($"model: {m_CharacterData.Model}, is there any animator: { m_CharacterData.Model.GetComponent<Animator>()}");
        Components.m_Animator = m_CharacterData.Model.GetComponent<Animator>();
        Components.m_Animator.avatar = m_CharacterData.CharacterAvatar;
        Components.m_Animator.runtimeAnimatorController = m_CharacterData.AnimationController;

        initiated = true;
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
        //Debug.LogError(debugMsg);
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
                item.Model.gameObject.SetActive(false);
                //Runner.Despawn(item.Model);
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

[Serializable]
public struct PlayerComponents
{
    //COMPONENTS
    public NetworkCharacterController m_NetworkCharacterController;
    public CharacterController m_CharacterController;
    public Animator m_Animator;
    public NetworkMecanimAnimator m_NetworkAnimator;

    //SCRIPTS
    public Player m_Player;
    public Health m_Health;
    public CharacterInputHandler m_CharacterInputHandler;
    public CombatController m_CombatController;
    public PlayerCommunication m_PlayerCommunication;
    public Collector m_Collector;
}
