using CardGame.Cards.Base;
using CardGame.Players;
using CardGame.Units.Base;

namespace Gameplay.Units
{
    public class KoboldSpearman : Unit
    {
        public KoboldSpearman(Card card, Player player, string instanceID = null) : base(card, player, instanceID)
        {
            _unitName = "Kobold Spearman";
            _health = _maxHealth = 2;
            _damage = 1;
            _race = UnitRace.Dragon;

            //description: 
            //max stacks: 4
            //cost: 1
        }
    }
}
