using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    GameManager Instance;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
    }

    void Start()
    {
        Countdown countDown = new Countdown(30f);
    }

    void StartRound()
    {
        Debug.Log("Starting Game");
        // switch all players' state from waiting to moving (for loop and .tag == "Player")
        // shuffle/randomize location of ear pieces
    }
}
