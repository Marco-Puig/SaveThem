using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdown : MonoBehaviour
{  
    [SerializedField]
    private float waitTime = 60.0f;
    


    private float time = 0;

    public Countdown(float countdownTime, GameObject )
    {
        waitTime = countdownTime; 
    }

    void Update()
    {
        time += Time.deltaTime;

        if (time >= waitTime)
        {
            GameManager.Instance.StartRound();
            Destroy(this);
        }
    }
}
