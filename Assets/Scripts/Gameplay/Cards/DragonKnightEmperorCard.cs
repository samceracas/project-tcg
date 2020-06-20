using CardGame.Cards.Base;
using CardGame.Players;
using Gameplay.Units;

namespace Gameplay.Cards
{
    public class DragonKnightEmperorCard : Card
    {
        public DragonKnightEmperorCard(Player player, string instanceID = null) : base(player, instanceID)
        {
            _cost = 3;
            _name = "Draconian Knight";
            _description = "";
            _cardType = CardType.Unit;
            _unit = new DragonKnightEmperor(instanceID);
            _health = _unit.Health;
            _attack = _unit.Damage;
        }
    }
}
