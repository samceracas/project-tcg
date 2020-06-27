using CardGame.Cards.Base;
using CardGame.Players;
using CardGame.Units.Base;
using Gameplay.Units;

namespace Gameplay.Cards
{
    public class ImperialDragonSovereignCard : Card
    {

        public ImperialDragonSovereignCard(Player player, string instanceID = null) : base(player, instanceID)
        {
            _cost = _defaultCost = 10;
            _name = "Imperial Dragon Sovereign";
            _description = "";
            _cardType = CardType.Unit;
            _unit = new ImperialDragonSovereign(this, player, instanceID);
            _health = _unit.Health;
            _attack = _unit.Damage;
            _race = UnitRace.Dragon;
        }
    }
}
