using CardGame.Units.Base;
using System;
using System.Collections.Generic;

namespace CardGame.Players
{
    public enum UnitSelectorState
    {
        Idle,
        Selecting
    }

    public class UnitSelector
    {
        protected List<Unit> _selected;
        protected UnitSelectorState _selectorState;
        protected int _targetCount;
        protected Action<List<Unit>> _actionToCall;
        protected Func<Unit, bool> _validator;
        protected float _delay;

        public Action EventSelectorReady;
        public Action EventSelectorComplete;
        public Action EventOnInvalidTarget;

        public UnitSelector()
        {
            _selected = new List<Unit>();
            _selectorState = UnitSelectorState.Idle;
            EventSelectorReady = () => { };
            EventOnInvalidTarget = () => { };
            EventSelectorComplete = () => { };
        }

        public UnitSelectorState State => _selectorState;

        public void StartSelect(int targetCount, Action<List<Unit>> toCall, Func<Unit, bool> validator, float delay = 0f)
        {
            _actionToCall = toCall;
            _targetCount = targetCount;
            _validator = validator;
            _selectorState = UnitSelectorState.Selecting;
            _delay = delay;

            EventSelectorReady?.Invoke();
        }

        public void EndSelect()
        {
            _selected = new List<Unit>();
            _selectorState = UnitSelectorState.Idle;
        }

        public void Select(Unit unit)
        {
            if (_selected.Count >= _targetCount && _selectorState == UnitSelectorState.Idle) return;

            if (!_validator(unit))
            {
                EventOnInvalidTarget?.Invoke();
                return;
            }

            _selected.Add(unit);

            if (_selected.Count >= _targetCount)
            {
                _actionToCall?.Invoke(_selected);
                EventSelectorComplete?.Invoke();
                EndSelect();
            }
        }
    }
}
