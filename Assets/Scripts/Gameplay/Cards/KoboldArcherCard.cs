using CardGame.Cards.Base;
using CardGame.Players;
using Gameplay.Units;

namespace Gameplay.Cards
{
    public class KoboldArcherCard : Card
    {

        public KoboldArcherCard(Player player, string instanceID = null) : base(player, instanceID)
        {
            _cost = 2;
            _name = "Kobold Archer";
            _description = "";
            _cardType = CardType.Unit;
            _unit = new KoboldArcher(instanceID);
            _health = _unit.Health;
            _attack = _unit.Damage;
        }
    }
}
