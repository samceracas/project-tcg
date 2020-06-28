using CardGame.Constants;
using CardGame.Events;
using CardGame.Units.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CardGame.Units.Abilities
{
    public class Taunt : UnitAbility
    {
        protected Interceptor _spawnInterceptor;

        public Taunt(Unit unit) : base(unit)
        {
            _id = "taunt";
            _isDuplicateAllowed = false;

            _spawnInterceptor = new Interceptor()
            {
                After = (state) =>
                {
                    Unit spawnedUnit = state.Arguments.unit;
                    state.Continue();

                    if (_unit.State == UnitState.Idle && spawnedUnit != _unit) return state;

                    if (spawnedUnit.Owner != _unit.Owner)
                    {
                        Debug.Log($"{_unit}:{_unit.Owner} is taunting {spawnedUnit}:{spawnedUnit.Owner}");
                        spawnedUnit.AttackWhiteListActive = true;
                        spawnedUnit.AddUnitToAttackWhiteList(_unit);
                    }

                    if (spawnedUnit == _unit)
                    {
                        EnableEnemyUnitWhiteList();
                    }
                    return state;
                }
            };
        }

        public override void Listen(bool applyDirectly = false)
        {
            if (_isActive) return;
            _isActive = true;

            EnableEnemyUnitWhiteList();
            _unit.Owner.Game.InterceptorService.RegisterInterceptor(InterceptorEvents.UnitSpawn, _spawnInterceptor);
        }

        public override void Revert()
        {
            if (!_isActive) return;
            _isActive = false;

            _unit.Owner.Game.InterceptorService.RemoveInterceptor(InterceptorEvents.UnitSpawn, _spawnInterceptor);
            ClearEnemyUnitWhiteList();
        }

        protected void EnableEnemyUnitWhiteList()
        {
            if (!_unit.Owner.Game.IsStarted || _unit.State == UnitState.Idle) return;

            foreach (KeyValuePair<string, Unit> pair in _unit.Owner.Opponent.UnitsOnField)
            {
                Debug.Log($"{_unit}:{_unit.Owner} is taunting {pair.Value}:{pair.Value.Owner}");
                pair.Value.AttackWhiteListActive = true;
                pair.Value.AddUnitToAttackWhiteList(_unit);
            }
        }

        protected void ClearEnemyUnitWhiteList()
        {
            var entry = _unit.Owner.GetUnitsWithAbility("taunt");
            bool hasAllyTaunt = entry.Count > 0;

            if (!hasAllyTaunt)
            {
                foreach (KeyValuePair<string, Unit> pair in _unit.Owner.Opponent.UnitsOnField)
                {
                    pair.Value.AttackWhiteListActive = false;
                    pair.Value.ClearAttackWhiteList();
                }
            } else
            {
                //remove this from white list
                foreach (KeyValuePair<string, Unit> pair in _unit.Owner.Opponent.UnitsOnField)
                {
                    pair.Value.RemoveUnitFromAttackWhiteList(_unit);
                }
            }
        }
    }
}
