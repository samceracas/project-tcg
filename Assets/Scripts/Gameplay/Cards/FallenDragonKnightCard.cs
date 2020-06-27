using CardGame.Cards.Base;
using CardGame.Players;
using CardGame.Units.Base;
using Gameplay.Units;

namespace Gameplay.Cards
{
    public class FallenDragonKnightCard : Card
    {

        public FallenDragonKnightCard(Player player, string instanceID = null) : base(player, instanceID)
        {
            _cost = _defaultCost = 2;
            _name = "Fallen Dragon Knight";
            _description = "";
            _cardType = CardType.Unit;
            _unit = new FallenDragonKnight(this, player, instanceID);
            _health = _unit.Health;
            _attack = _unit.Damage;
            _race = UnitRace.Dragon;
        }
    }
}
