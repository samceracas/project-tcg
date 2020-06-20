using System.Collections.Generic;

namespace CardGame.Effectors.Base
{
    public class EffectorStack
    {
        protected List<Effector> _effectors;
        protected int _currentEffectorIndex;

        public EffectorStack()
        {
            _effectors = new List<Effector>();
            _currentEffectorIndex = -1;
        }

        public int CurrentEffectorIndex => _currentEffectorIndex;

        public void AddEffector(Effector effector)
        {
            _effectors.Add(effector);
        }

        public void ApplyEffector(Effector effector)
        {
            AddEffector(effector);
            MoveToLatest();
        }

        public void MoveTo(int moveIndex)
        {
            if ((moveIndex == _currentEffectorIndex && _effectors.Count > 1) ||
                moveIndex >= _effectors.Count || moveIndex < 0) return;

            bool forward = moveIndex >= _currentEffectorIndex;
            int i = _currentEffectorIndex;
            int modifier = forward ? 1 : -1;
            i += modifier;

            while (forward ? i <= moveIndex : i >= moveIndex)
            {
                if (forward)
                {
                    //UnityEngine.Debug.Log("Applying effector " + _effectors[i].ID);
                    _effectors[i].Apply();
                }
                else
                {
                    //UnityEngine.Debug.Log("Reverting effector " + _effectors[i].ID);
                    _effectors[i].Revert();
                }
                i += modifier;
            }

            _currentEffectorIndex = moveIndex;
        }

        public void MoveToLatest()
        {
            MoveTo(_effectors.Count - 1);
        }

        public void MoveToInitial()
        {
            MoveTo(0);
        }
    }
}