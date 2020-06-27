using CardGame.Cards.Base;
using CardGame.Players;
using CardGame.Units.Base;

namespace Gameplay.Units
{
    public class KoboldSpellCaster : Unit
    {
        public KoboldSpellCaster(Card card, Player player, string instanceID = null) : base(card, player, instanceID)
        {
            _unitName = "Kobold Spell Caster";
            _health = _maxHealth = 1;
            _damage = 2;
            _race = UnitRace.Dragon;

            //Draw a random spell card
            //max stacks: 4
            //cost: 1
        }
    }
}
