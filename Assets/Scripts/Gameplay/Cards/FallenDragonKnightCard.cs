using CardGame.Cards.Base;
using CardGame.Players;
using Gameplay.Units;

namespace Gameplay.Cards
{
    public class FallenDragonKnightCard : Card
    {

        public FallenDragonKnightCard(Player player, string instanceID = null) : base(player, instanceID)
        {
            _cost = 5;
            _name = "Fallen Dragon Knight";
            _description = "";
            _cardType = CardType.Unit;
            _unit = new FallenDragonKnight(instanceID);
            _health = _unit.Health;
            _attack = _unit.Damage;
        }
    }
}
