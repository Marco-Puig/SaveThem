using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class CreateLobby : MonoBehaviourPunCallbacks
{
    [Header("Photon Settings")]
    [SerializeField] string gameVersion = "1";
    [SerializeField] byte maxPlayers = 4;

    [Header("Input References")]
    [SerializeField] TMP_InputField lobbyCode;
    [SerializeField] TMP_InputField nameInput;

    private void Awake()
    {
        // This makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = gameVersion;
    }

    public void LobbyOnConfirm()
    {
        if (!string.IsNullOrEmpty(lobbyCode.text) && !string.IsNullOrWhiteSpace(nameInput.text))
        {
            // Save Player Name and Lobby Code to a serialized save on /AppData
            PlayerPrefs.SetString("PlayerName", nameInput.text);
            PlayerPrefs.SetString("LobbyCode", lobbyCode.text);

            // Connect to Room and save the PlayerName to the Network
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.NickName = PlayerPrefs.GetString("PlayerName");
                PhotonNetwork.JoinRoom(PlayerPrefs.GetString("LobbyCode"));
            }
        }
    }

    // If Player fails to Join a Room, that just means it doesn't exist yet, so create one with the inputted LobbyCode.
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(PlayerPrefs.GetString("LobbyCode"), new RoomOptions { MaxPlayers = maxPlayers });
    }
}
