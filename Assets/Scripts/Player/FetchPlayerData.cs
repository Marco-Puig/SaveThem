using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class FetchPlayerData : MonoBehaviourPunCallbacks
{
    [SerializeField]
    TMP_Text playerName;

    [SerializeField]
    TMP_Text currentLobby;

    void Start()
    {
        playerName.text = GetName();
        currentLobby.text = GetLobbyCode();
    }

    string GetName()
    {
        PlayerPrefs.SetString("PlayerName", photonView.Owner.NickName);
        return PlayerPrefs.GetString("PlayerName");
    }

    string GetLobbyCode()
    {
        return "Lobby Code: " + PlayerPrefs.GetString("LobbyCode");
    }

    // in the future when fetching data make sure to use try catch
}
