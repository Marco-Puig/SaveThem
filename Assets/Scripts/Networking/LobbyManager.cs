using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    // Singleton design, instance
    public static LobbyManager Instance { get; private set; }

    // When in game, refer to these
    [Header("In Game")]
    [SerializeField] GameObject LobbyManagerUI;
    [SerializeField] string gameScene = "TestScene";

    [Header("Instances")]
    // Spawn unique player object to represents the player themselves in game on the network / lobby instance
    public GameObject playerPrefab;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null) { Instance = this; }

        DontDestroyOnLoad(this);
    }

    #region Public Methods

    public void LeaveRoom()
    {
        LobbyManagerUI.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }

    public void MakeLobbyUnJoinable()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
    }

    public void ShutdownLobby()
    {
        // same as LeaveRoom(), but i want this function to add anything additional in the future.
        Instance.LeaveRoom();
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
        PhotonNetwork.LoadLevel(gameScene);
        LobbyManagerUI.SetActive(true);
    }

    #endregion

    #region Photon Callbacks

    public override void OnJoinedRoom()
    {
        // Instantiate player
        if (PlayerManager.LocalPlayerInstance == null)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(Random.Range(-6f, 9f), -2.5f, 0f), Quaternion.identity, 0);
        }

        // We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            // Load the Room Level.
            PhotonNetwork.LoadLevel(1);
        }

        LobbyManagerUI.SetActive(true);
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

    // Called when the local player left the room. We need to load the Menu/Lobby scene.
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    #endregion

}
