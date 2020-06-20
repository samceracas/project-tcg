using CardGame.Cards.Base;
using CardGame.Players;
using Gameplay.Units;

namespace Gameplay.Cards
{
    public class KoboldSpearmanCard : Card
    {
        public KoboldSpearmanCard(Player player, string instanceID = null) : base(player, instanceID)
        {
            _cost = 1;
            _name = "Kobold Spearman";
            _description = "";
            _cardType = CardType.Unit;
            _unit = new KoboldSpearman(instanceID);
            _health = _unit.Health;
            _attack = _unit.Damage;
        }
    }
}
