using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdown : MonoBehaviour
{ 
    private float waitTime = 60.0f, time = 0;

    void Update()
    {
        Tick();
    }

    void Tick()
    {
        time += Time.deltaTime;

        if (time >= waitTime)
        {
            GameManager.Instance.StartRound();
            Destroy(this);
        }
    }
}
