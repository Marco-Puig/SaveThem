using System;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPun
{
    #region Initialize
    public static GameManager Instance { get; private set; }

    [Header("Round Management")]
    [SerializeField] Countdown countdown;
    [SerializeField] float waitTime = 60f;
    [SerializeField] GameObject earPiecePrefab;

    [Header("Lobby Setup")]
    [SerializeField] GameObject lobby;
    [SerializeField] GameObject mainRoom;
    [SerializeField] bool useLobby = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null) { Instance = this; }
    }

    void Start()
    {
        // setup countdown for lobby wait and rounds
        if (photonView.IsMine)
        {
            countdown = this.gameObject.GetComponent<Countdown>();
            photonView.RPC("LobbyWait", RpcTarget.AllBuffered);
        }
    }

    #endregion

    # region Private Methods
    [PunRPC]

    void LobbyWait()
    {
        // wait for players to join for 60 seconds
        countdown.Setup(WaitForFirstRound, waitTime, "Waiting for more players to join:");
    }

    void WaitForFirstRound()
    {
        // Once the game starts, make the lobby unjoinable
        LobbyManager.Instance.MakeLobbyUnJoinable();

        // Have players wait
        SetPlayersState("Waiting");

        // Transition to Start Wait
        countdown.Setup(StartRound, 10.0f, "Next round starts in");

        // make sure that lobby room is now game room
        if (useLobby)
        {
            lobby.SetActive(false);
            mainRoom.SetActive(true);
        }

    }

    void StartRound()
    {
        // Spawn EARPIECES
        SpawnEarpiecePickups();

        // Allow players to move (This is the main gameplay part)
        SetPlayersState("Moving");

        // at round end, call WaitForRound
        countdown.Setup(WaitForRound, 30.0f, "Round ends in");
    }

    void WaitForRound()
    {
        // Have players wait, in the meantime, check for anyone without an earpiece
        SetPlayersState("Waiting");
        CheckForLoser();

        // Transition to Start Wait
        countdown.Setup(StartRound, 10.0f, "Next round starts in");
    }

    void SetPlayersState(string wantedState)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            switch (wantedState)
            {
                case "Moving":
                    players[i].GetComponent<Player>().currentState = players[i].GetComponent<Player>().Moving;
                    break;
                case "Waiting":
                    players[i].GetComponent<Player>().currentState = players[i].GetComponent<Player>().Waiting;
                    break;
                default:
                    Debug.LogError("Invalid state");
                    break;
            }
        }
    }

    int GetPlayerCount()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        return players.Length;
    }

    void CheckForLoser()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int playersLeft = 0;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().hasEarpiece)
            {
                players[i].GetComponent<Player>().currentState = players[i].GetComponent<Player>().Winner;
                playersLeft++;
            }
            else
            {
                players[i].GetComponent<Player>().currentState = players[i].GetComponent<Player>().Loser;
            }
        }

        if (playersLeft <= 1)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        // check for winner
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            // if player has earpiece, they are the winner
            if (players[i].GetComponent<Player>().hasEarpiece)
            {
                players[i].GetComponent<Player>().currentState = players[i].GetComponent<Player>().Winner;
                countdown.Setup(LobbyManager.Instance.ShutdownLobby, 30.0f, "A WINNER HAS BEEN DECIDED! Ending game in");
                break;
            }
        }

        // if no winner, restart game
        countdown.Setup(StartRound, 25.0f, "NO WINNER! Rematch game starts in");
    }

    void SpawnEarpiecePickups()
    {
        // instantiate pickup prefabs, always make it one less than total players (cuz musical chairs)
        for (int i = 0; i < GetPlayerCount() - 1; i++)
        {
            var position = new Vector3(UnityEngine.Random.Range(-7.29f, 8.41f), 0, UnityEngine.Random.Range(-4.46f, -1.7f));
            // spawn earpiece
            Debug.Log("Spawning earpiece at: " + position);
            Instantiate(earPiecePrefab, position, Quaternion.identity);
        }
    }

    # endregion
}
