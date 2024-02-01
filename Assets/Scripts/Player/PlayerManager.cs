using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Just making the player non-destroyable for PUN to keep track between scenes.

public class PlayerManager : MonoBehaviourPunCallbacks 
{
    public static GameObject LocalPlayerInstance;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
        }

        DontDestroyOnLoad(gameObject);
    }

}