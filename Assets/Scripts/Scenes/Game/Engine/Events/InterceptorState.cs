using System.Dynamic;

namespace CardGame.Events
{
    public enum InterceptorStateResponse
    {
        Continue,
        RemoveAndContinue,
        Deny
    };

    public class InterceptorState
    {
        protected ExpandoObject _arguments;

        public InterceptorState()
        {
            _arguments = new ExpandoObject();
        }

        public dynamic Arguments { get => _arguments; set => _arguments = value; }
        public InterceptorStateResponse State { get; private set; }

        public void Deny()
        {
            State = InterceptorStateResponse.Deny;
        }

        public void Continue()
        {
            State = InterceptorStateResponse.Continue;
        }

        public void RemoveAndContinue()
        {
            State = InterceptorStateResponse.RemoveAndContinue;
        }
    }
}
