using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public float MaxTime = 60;
    public float timeLeft;

    public UnityEvent onTimeOut;

    void Start()
    {
        Restart();    
    }

    public void Restart() 
    {
        timeLeft = MaxTime;
    }

    
    void Update()
    {
        
        if (timeLeft <= 0)
        {
            timeLeft = 0;
            onTimeOut.Invoke();
        }
        else 
        {
            timeLeft -= Time.deltaTime;
        }
    }
}
