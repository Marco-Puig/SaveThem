using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Countdown : MonoBehaviour
{ 
    private float waitTime = 60.0f, time = 0;
    public TMP_Text timerText;

    void Update()
    {
        Tick();
    }

    void Tick()
    {
        time += Time.deltaTime;
        timerText.text = time;

        if (time >= waitTime)
        {
            GameManager.Instance.StartRound();
            Destroy(this);
        }
    }
}
