using System;
using UnityEngine;

public class Timer
{
    private enum TimerState
    {
        Running,
        Paused,
        Finished,
    }

    private float currentTime = 0f;
    private float endTime;
    public event Action OnTimerEnd;

    private TimerState _state = TimerState.Paused;

    private TimerState State {
        get { return _state; }
        set {
            if (_state != TimerState.Running && value == TimerState.Running)
            {
                TimerManager.Instance.OnTimerUpdate += UpdateTimer;
            }
            else if (_state == TimerState.Running && value != TimerState.Running)
            {
                TimerManager.Instance.OnTimerUpdate -= UpdateTimer;
            }

            _state = value;
        }
    }
    public float EndTime
    {
        get { return endTime; }
        set {
            if (currentTime > value) { State = TimerState.Finished; }
            else if (IsFinished && currentTime < value) { State = TimerState.Paused; }

            endTime = value;
        }
    }
    public float TimeLeft { get { return endTime - currentTime; } }
    public bool IsRunning { get { return State == TimerState.Running; } }
    public bool IsFinished { get { return State == TimerState.Finished; } }

    public Timer(float endTime, Action callback = null, bool autoStart = true) 
    {
        this.endTime = endTime;

        if (autoStart) { State = TimerState.Running; }
        else { State = TimerState.Paused; }

        if (callback != null) { OnTimerEnd += callback; }
    }

    public void Start()
    {
        State = TimerState.Running;
    }

    public void UpdateTimer(float delta) 
    {
        if (State != TimerState.Running) { return; }

        currentTime += delta;
        if (currentTime >= endTime)
        {
            State = TimerState.Finished;

            try { OnTimerEnd?.Invoke(); }
            catch (MissingReferenceException) { }
        }
    }

    public void Reset(bool autoStart = true)
    {
        currentTime = 0f;
        if (autoStart) { State = TimerState.Running; }
        else { State = TimerState.Paused; }
       
    }

    public void Pause() 
    {
        State = TimerState.Paused;
    }
}
