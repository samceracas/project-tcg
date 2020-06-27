using CardGame.Cards.Base;
using CardGame.Players;
using CardGame.Units.Base;
using Gameplay.Units;

namespace Gameplay.Cards
{
    public class KoboldSpearmanCard : Card
    {
        public KoboldSpearmanCard(Player player, string instanceID = null) : base(player, instanceID)
        {
            _cost = _defaultCost = 1;
            _name = "Kobold Spearman";
            _description = "";
            _cardType = CardType.Unit;
            _unit = new KoboldSpearman(this, player, instanceID);
            _health = _unit.Health;
            _attack = _unit.Damage;
            _race = UnitRace.Dragon;
        }
    }
}
