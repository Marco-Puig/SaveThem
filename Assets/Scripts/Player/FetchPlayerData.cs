using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FetchPlayerData : MonoBehaviour
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
        return PlayerPrefs.GetString("PlayerName");
    }

    string GetLobbyCode()
    {
        return "Lobby Code: " + PlayerPrefs.GetString("LobbyCode");
    }

    // in the future when fetching data make sure to use try catch
}
