using System;

namespace CardGame.Events
{
    public class Interceptor
    {
        public bool AllowRunInSimulation = true;
        public int Priority = 1;
        public Func<InterceptorState, InterceptorState> After = (x) => x;
        public Func<InterceptorState, InterceptorState> Before = (x) => x;
    }
}
