using Library.Utils.Unity;
using System;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;

namespace Library.Timer
{
    public class SingletonTimer : Singleton<SingletonTimer>
    {
        protected ConcurrentDictionary<string, int> _runningTimerIds;

        /** Events **/
        public Action<string, int> EventOnRepeatTimerEnd = (a, b) => { };

        protected SingletonTimer()
        {
            _runningTimerIds = new ConcurrentDictionary<string, int>();
        }

        public void DelayedInvoke(Action method, float invokeAfter)
        {
            StartCoroutine(RunCoroutine(method, invokeAfter));
        }

        public void StopTimer(string timerId)
        {
            int count = 0;
            _runningTimerIds.TryRemove(timerId, out count);
        }

        public string InvokeRepeating(Action method, float startTime, float endTime, int endAfterCount = -1)
        {
            string randId = System.Guid.NewGuid().ToString();
            _runningTimerIds.TryAdd(randId, 0);
            StartCoroutine(RepeatingCoroutine(method, randId, startTime, endTime, endAfterCount));
            return randId;
        }

        private IEnumerator RunCoroutine(Action method, float invokeAfter)
        {
            yield return new WaitForSeconds(invokeAfter);
            method();
        }

        private IEnumerator RepeatingCoroutine(Action method, string timerId, float startTime, float endTime, int endAfterCount = -1)
        {
            yield return new WaitForSeconds(startTime);
            if (_runningTimerIds.ContainsKey(timerId))
            {
                _runningTimerIds[timerId]++;
                method();
            }
            while (_runningTimerIds.ContainsKey(timerId))
            {
                if (_runningTimerIds[timerId] >= endAfterCount)
                {
                    int lastCount = 0;
                    _runningTimerIds.TryRemove(timerId, out lastCount);
                    EventOnRepeatTimerEnd.Invoke(timerId, lastCount);
                    yield break;
                }
                yield return new WaitForSeconds(endTime);
                _runningTimerIds[timerId]++;
                method();
            }
            EventOnRepeatTimerEnd.Invoke(timerId, -1);
            yield break;
        }
    }
}