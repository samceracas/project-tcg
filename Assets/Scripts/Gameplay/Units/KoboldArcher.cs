using CardGame.Cards.Base;
using CardGame.Players;
using CardGame.Units.Base;

namespace Gameplay.Units
{
    public class KoboldArcher : Unit
    {
        public KoboldArcher(Card card, Player player, string instanceID = null) : base(card, player, instanceID)
        {
            _unitName = "Kobold Archer";
            _health = _maxHealth = 3;
            _damage = 2;
            _race = UnitRace.Dragon;

            //Nimble: can attack twice in 1 turn
            //max stacks: 4
            //cost: 2
        }
    }
}
