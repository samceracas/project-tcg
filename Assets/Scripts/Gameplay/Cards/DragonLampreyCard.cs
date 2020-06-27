using CardGame.Cards.Base;
using CardGame.Players;
using CardGame.Units.Base;
using Gameplay.Units;

namespace Gameplay.Cards
{
    public class DragonLampreyCard : Card
    {
        public DragonLampreyCard(Player player, string instanceID = null) : base(player, instanceID)
        {
            _cost = _defaultCost = 1;
            _name = "Dragon Lamprey";
            _description = "<b>Lifesteal</b>";
            _cardType = CardType.Unit;
            _unit = new DragonLamprey(player, this, instanceID);
            _health = _unit.Health;
            _attack = _unit.Damage;
            _race = UnitRace.Dragon;
        }
    }
}
