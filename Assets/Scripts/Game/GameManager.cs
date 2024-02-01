using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    #region Initialize
    public static GameManager Instance { get; private set;}
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }

    #endregion

    #region Public Methods
    public void StartRound()
    {
        Debug.Log("Starting Game");
        // switch all players' state from waiting to moving (for loop and .tag == "Player")
        // shuffle/randomize location of ear pieces
    }

    #endregion
}
