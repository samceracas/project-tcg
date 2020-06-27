using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownTimer : MonoBehaviour
{
    public class Entry
    {
        protected float _totalTime = 0f;
        protected float _currentTime;
        protected bool _isFinished = false, _isPaused = false;

        public Action<Entry> OnComplete = (a) => { };
        public Action<Entry> OnUpdate = (a) => { };


        public float CountdownTime {
            get {
                return _totalTime;
            }
            set {
                _totalTime = value;
            }
        }

        public int TimeInSeconds => Mathf.RoundToInt(_currentTime);
        public float CurrentTime => _currentTime;
        public bool IsFinished => _isFinished;

        public void Update(float deltaTime)
        {
            if (_isPaused || _isFinished) return;

            _currentTime -= deltaTime;
            OnUpdate?.Invoke(this);
            if (_currentTime <= 0f && !_isFinished)
            {
                _currentTime = 0f;
                _isFinished = true;
                OnComplete?.Invoke(this);
            }
        }

        public void Pause()
        {
            _isPaused = true;
        }

        public void Resume()
        {
            _isPaused = false;
        }

        public void Stop(bool callComplete = false)
        {
            _currentTime = 0f;
            _isFinished = true;

            if (callComplete)
            {
                OnComplete?.Invoke(this);
            }
        }

        public void Reset(bool clearListeners = false)
        {
            if (clearListeners)
            {
                ClearListeners();
            }

            _currentTime = _totalTime;
            _isFinished = false;
        }

        public void ClearListeners()
        {
            OnComplete = (a) => { };
            OnUpdate = (a) => { };
        }
    }

    protected static CountdownTimer _instance;

    public static CountdownTimer Instance => _instance;


    protected List<CountdownTimer.Entry> _countdownInstances;

    private void Start()
    {
        _countdownInstances = new List<Entry>();
        _instance = this;
    }

    private void Update()
    {
        foreach (Entry entry in _countdownInstances)
        {
            entry.Update(Time.deltaTime);
        }
    }

    public int AddEntry(CountdownTimer.Entry entry)
    {
        entry.Reset();

        _countdownInstances.Add(entry);
        return _countdownInstances.Count;
    }

    public void RemoveEntry(CountdownTimer.Entry entry)
    {
        _countdownInstances.Remove(entry);
    }
}
