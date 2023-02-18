using System;
using UnityEngine;

public class TimerManager : Singleton<TimerManager>
{
    public event Action<float> OnTimerUpdate;

    public void FixedUpdate()
    {
        OnTimerUpdate?.Invoke(Time.fixedDeltaTime);
    }
}
