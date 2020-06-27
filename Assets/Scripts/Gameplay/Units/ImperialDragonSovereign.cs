using CardGame.Cards.Base;
using CardGame.Players;
using CardGame.Units.Base;

namespace Gameplay.Units
{
    public class ImperialDragonSovereign : Unit
    {
        public ImperialDragonSovereign(Card card, Player player, string instanceID = null) : base(card, player, instanceID)
        {
            //generate a name here: https://donjon.bin.sh/fantasy/name
            _unitName = "Imperial Dragon Sovereign";
            _health = _maxHealth = 11;
            _damage = 11;
            _race = UnitRace.Dragon;

            //cost: 10
            //active: Set all enemy dragons to 1/1
            //max stacks: 1
        }
    }
}
