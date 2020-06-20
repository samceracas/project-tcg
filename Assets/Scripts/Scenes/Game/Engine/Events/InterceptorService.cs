using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace CardGame.Events
{
    public class InterceptorService
    {
        private Dictionary<string, List<Interceptor>> _interceptors;
        private readonly object _interceptorsLock = new object();
        private bool _isSimulated = false;
        public InterceptorService()
        {
            _interceptors = new Dictionary<string, List<Interceptor>>();
        }

        public bool IsSimulated { get => _isSimulated; set => _isSimulated = value; }

        public void RegisterInterceptor(string eventType, Interceptor interceptor)
        {
            lock (_interceptorsLock)
            {
                if (!_interceptors.ContainsKey(eventType))
                {
                    _interceptors.Add(eventType, new List<Interceptor>());
                }
                _interceptors[eventType].Add(interceptor);
                _interceptors[eventType].OrderBy(entry => entry.Priority);
            }
        }

        public void RemoveInterceptor(string eventType, Interceptor interceptor)
        {
            lock (_interceptorsLock)
            {
                if (_interceptors.ContainsKey(eventType))
                {
                    _interceptors[eventType].Remove(interceptor);
                }
            }
        }

        public void RunInterceptor(string eventType, Action toExecute, ExpandoObject parameters)
        {

            lock (_interceptorsLock)
            {
                if (_interceptors.ContainsKey(eventType))
                {
                    InterceptorStateResponse overallState = InterceptorStateResponse.Continue;
                    overallState = RunBefore(eventType, parameters);
                    if (overallState != InterceptorStateResponse.Deny)
                    {
                        toExecute();
                    }
                    RunAfter(eventType, parameters);
                }
                else
                {
                    //no interceptors, execute the action
                    toExecute();
                }
            }

        }

        private InterceptorStateResponse RunBefore(string eventType, ExpandoObject parameters)
        {
            lock (_interceptorsLock)
            {
                if (!_interceptors.ContainsKey(eventType)) return InterceptorStateResponse.Deny;
                InterceptorStateResponse overallState = InterceptorStateResponse.Continue;
                List<Interceptor> toRemove = new List<Interceptor>();

                foreach (Interceptor interceptor in _interceptors[eventType])
                {
                    if (IsSimulated && !interceptor.AllowRunInSimulation) continue;

                    InterceptorState beforeState = new InterceptorState();
                    beforeState.Arguments = parameters;
                    beforeState = interceptor.Before(beforeState);
                    overallState = beforeState.State;
                    //break the execution for other interceptors
                    if (beforeState.State == InterceptorStateResponse.Deny)
                    {
                        overallState = InterceptorStateResponse.Deny;
                        break;
                    }

                    if (beforeState.State == InterceptorStateResponse.RemoveAndContinue)
                    {
                        toRemove.Add(interceptor);
                    }
                }

                foreach (Interceptor interceptorInstance in toRemove)
                {
                    _interceptors[eventType].Remove(interceptorInstance);
                }
                return overallState;
            }
        }

        private InterceptorStateResponse RunAfter(string eventType, ExpandoObject parameters)
        {
            lock (_interceptorsLock)
            {
                if (!_interceptors.ContainsKey(eventType)) return InterceptorStateResponse.Deny;

                InterceptorStateResponse overallState = InterceptorStateResponse.Continue;
                List<Interceptor> toRemove = new List<Interceptor>();
                List<Interceptor> interceptorList = _interceptors[eventType];
                foreach (Interceptor interceptor in interceptorList)
                {

                    if (IsSimulated && !interceptor.AllowRunInSimulation) continue;

                    InterceptorState afterState = new InterceptorState();
                    afterState.Arguments = parameters;
                    afterState = interceptor.After(afterState);
                    overallState = afterState.State;
                    //break the execution for other interceptors
                    if (afterState.State == InterceptorStateResponse.Deny)
                    {
                        overallState = InterceptorStateResponse.Deny;
                        break;
                    }

                    if (afterState.State == InterceptorStateResponse.RemoveAndContinue)
                    {
                        toRemove.Add(interceptor);
                    }
                }

                foreach (Interceptor interceptorInstance in toRemove)
                {
                    interceptorList.Remove(interceptorInstance);
                }
                return overallState;
            }
        }

        public List<Interceptor> GetInterceptors(string eventType)
        {
            return _interceptors.ContainsKey(eventType) ? _interceptors[eventType] : null;
        }

        public void ClearInterceptors()
        {
            lock (_interceptorsLock)
            {
                _interceptors = new Dictionary<string, List<Interceptor>>();
            }
        }
    }
}
