using CardGame.Cards.Base;
using CardGame.Players;
using Gameplay.Units;

namespace Gameplay.Cards
{
    public class HarmlessDragonEggCard : Card
    {
        public HarmlessDragonEggCard(Player player, string instanceID = null) : base(player, instanceID)
        {
            _cost = 3;
            _name = "Harmless Dragon Egg";
            _description = "It's a harmless dragon egg, what could it possibly do? Last Breath: Summon an angry dragon momma 6/6";
            _cardType = CardType.Unit;
            _unit = new HarmlessDragonEgg(instanceID);
            _health = _unit.Health;
            _attack = _unit.Damage;
        }
    }
}
