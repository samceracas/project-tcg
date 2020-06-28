using CardGame.Cards.Base;
using CardGame.Constants;
using CardGame.Events;
using CardGame.Players;
using CardGame.Units.Abilities;
using CardGame.Units.Base;
using UnityEngine;

namespace Gameplay.Units
{
    public class DragonLamprey : Unit
    {

        public DragonLamprey(Player player, Card card, string instanceID = null) : base(card, player, instanceID)
        {
            _unitName = "Dragon Lamprey";
            _health = _maxHealth = 1;
            _damage = 2;
            _race = UnitRace.Dragon;
            //max stacks: 4
            //lifesteal
            //cost: 1

            Lifesteal lifesteal = new Lifesteal(this);
            lifesteal.AddToUnit();
        }
    }
}
