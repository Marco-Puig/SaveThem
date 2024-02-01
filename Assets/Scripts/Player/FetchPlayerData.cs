using System;
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
        try 
        {
            playerName.text = GetName();
            currentLobby.text = GetLobbyCode();            
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
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
}
