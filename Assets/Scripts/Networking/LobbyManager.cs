using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Linq;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("In Game")]
    [SerializeField]
    GameObject LobbyManagerUI;

    public void Awake()
    {
        GameObject[] lobbyManagers = GameObject.FindGameObjectsWithTag("LobbyManager");

        if (lobbyManagers.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    #region Public Methods

    public void LeaveRoom()
    {
        LobbyManagerUI.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }

    #endregion

    #region Private Methods

    private void LoadRoom()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but you are not the master Client");
            return;
        }
        Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
        PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    #endregion

    #region Photon Callbacks

    public override void OnJoinedRoom()
    {
        // We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            // Load the Room Level.
            LobbyManagerUI.SetActive(true);
            PhotonNetwork.LoadLevel(1); // change to 1
        }
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", otherPlayer.NickName); // not seen if you're the player connecting

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            LoadRoom();
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", otherPlayer.NickName); // seen when other disconnects

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            LoadRoom();
        }
    }

    /// Called when the local player left the room. We need to load the Menu/Lobby scene.
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    #endregion

}
