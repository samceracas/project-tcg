using CardGame.Cards.Base;
using CardGame.Players;
using CardGame.Units.Abilities;
using CardGame.Units.Base;

namespace Gameplay.Units
{
    public class DragonKnightEmperor : Unit
    {
        public DragonKnightEmperor(Card card, Player player, string instanceID = null) : base(card, player, instanceID)
        {
            _unitName = "Dragon Knight Emperor";
            _health = _maxHealth = 4;
            _damage = 3;
            _race = UnitRace.Dragon;

            //passive: increase dragons health and attack by 2/2
            //cost: 5
            //max stacks: 2

            Taunt taunt = new Taunt(this);
            taunt.AddToUnit();
        }
    }
}
