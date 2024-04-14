using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;

public class SessionInfoListUIItem : MonoBehaviour
{
    public TextMeshProUGUI SessionNameText;
    public TextMeshProUGUI PlayerCountText;
    public Button JoinButton;

    SessionInfo sessionInfo;

    public event Action<SessionInfo> OnJoinSession;

    public void SetInformation(SessionInfo pSessionInfo)
    {
        this.sessionInfo = pSessionInfo;

        SessionNameText.text = pSessionInfo.Name;
        PlayerCountText.text = $"{pSessionInfo.PlayerCount.ToString()} / {pSessionInfo.MaxPlayers.ToString()}";

        bool isJoinButtonActive = true;
        if (pSessionInfo.PlayerCount >= pSessionInfo.MaxPlayers)
            isJoinButtonActive = false;

        JoinButton.gameObject.SetActive(isJoinButtonActive);
    }

    public void OnClick()
    {
        OnJoinSession?.Invoke(sessionInfo);
    }

}
