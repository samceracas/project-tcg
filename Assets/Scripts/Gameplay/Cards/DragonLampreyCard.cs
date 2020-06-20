using CardGame.Cards.Base;
using CardGame.Players;
using Gameplay.Units;

namespace Gameplay.Cards
{
    public class DragonLampreyCard : Card
    {

        public DragonLampreyCard(Player player, string instanceID = null) : base(player, instanceID)
        {
            _cost = 1;
            _name = "Dragon Lamprey";
            _description = "";
            _cardType = CardType.Unit;
            _unit = new DragonLamprey(instanceID);
            _health = _unit.Health;
            _attack = _unit.Damage;
        }
    }
}
