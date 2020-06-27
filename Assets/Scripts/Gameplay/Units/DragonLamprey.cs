using CardGame.Cards.Base;
using CardGame.Constants;
using CardGame.Events;
using CardGame.Players;
using CardGame.Units.Base;
using UnityEngine;

namespace Gameplay.Units
{
    public class DragonLamprey : Unit
    {
        protected Interceptor _lifestealInterceptor;

        public DragonLamprey(Player player, Card card, string instanceID = null) : base(card, player, instanceID)
        {
            _unitName = "Dragon Lamprey";
            _health = _maxHealth = 1;
            _damage = 2;
            _race = UnitRace.Dragon;
            //max stacks: 4
            //lifesteal
            //cost: 1
        }

        public override void ReadyEvents()
        {
            if (_lifestealInterceptor == null)
            {
                _lifestealInterceptor = new Interceptor()
                {
                    After = (state) =>
                    {
                        Unit dealer = state.Arguments.dealer;
                        if (dealer == this)
                        {
                            _owner.Heal(this, Damage);
                        }

                        state.Continue();
                        return state;
                    }
                };
                _owner.Game.InterceptorService.RegisterInterceptor(InterceptorEvents.UnitAttack, _lifestealInterceptor);
            }
            base.ReadyEvents();
        }

        public override void ClearEvents()
        {
            _owner.Game.InterceptorService.RemoveInterceptor(InterceptorEvents.UnitAttack, _lifestealInterceptor);
            _lifestealInterceptor = null;
            base.ClearEvents();
        }
    }
}
