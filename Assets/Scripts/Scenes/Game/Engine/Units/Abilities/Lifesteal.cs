using CardGame.Constants;
using CardGame.Events;
using CardGame.Units.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame.Units.Abilities
{
    public class Lifesteal : UnitAbility
    {

        protected Interceptor _lifestealInterceptor;

        public Lifesteal(Unit unit) : base(unit)
        {
            _id = "lifesteal";
            _isDuplicateAllowed = false;

            _lifestealInterceptor = new Interceptor()
            {
                After = (state) =>
                {
                    state.Continue();
                    if (_unit.State == UnitState.Idle) return state;

                    Unit dealer = state.Arguments.dealer;
                    if (dealer == _unit)
                    {
                        _unit.Owner.Heal(_unit, _unit.Damage);
                    }
                    return state;
                }
            };
        }

        public override void Listen(bool applyDirectly = false)
        {
            if (_isActive) return;
            _isActive = true;

            _unit.Owner.Game.InterceptorService.RegisterInterceptor(InterceptorEvents.UnitAttack, _lifestealInterceptor);
        }

        public override void Revert()
        {
            if (!_isActive) return;
            _isActive = false;

            _unit.Owner.Game.InterceptorService.RemoveInterceptor(InterceptorEvents.UnitAttack, _lifestealInterceptor);
        }
    }
}
