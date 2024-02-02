using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Countdown : MonoBehaviourPun
{ 
    private float waitTime = 60.0f;
    private string timeMessage;
    public TMP_Text timerText;
    public delegate void FunctionCall();
    [HideInInspector] public FunctionCall functionCall;

    // This noooot constructor will allow us to re-use countdown for other than lobby wait.
    public void Setup(FunctionCall functionToCall, float timer, string timeMessage)
    {
        this.functionCall = functionToCall;
        this.waitTime = timer;
        this.timeMessage = timeMessage;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("Tick", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void Tick()
    {
        waitTime -= Time.deltaTime;
        timerText.text = timeMessage + " " + ((int)waitTime).ToString() + " seconds";

        if (waitTime <= 0)
        {
            functionCall?.Invoke();
        }
    }
}
