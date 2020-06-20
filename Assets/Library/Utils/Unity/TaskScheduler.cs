using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public delegate void UnityTask();

public class TaskScheduler : MonoBehaviour
{
    protected static TaskScheduler _instance;
    public static TaskScheduler Instance => _instance;

    private Queue<UnityTask> _taskQueue;
    private readonly object _taskLock = new object();

    // Start is called before the first frame update
    void Start()
    {
        _taskQueue = new Queue<UnityTask>();
        _instance = this;

    }

    public void Queue(UnityTask task)
    {
        _taskQueue.Enqueue(task);
    }

    // Update is called once per frame
    void Update()
    {
        if (Monitor.TryEnter(_taskLock, 5000))
        {
            try
            {
                if (_taskQueue.Count > 0)
                {
                    _taskQueue.Dequeue()();
                }
            }
            finally
            {
                Monitor.Exit(_taskLock);
            }
        }
        else
        {
            Debug.LogError("Failed to acquire lock!");
        }
    }
}
