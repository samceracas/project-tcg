using CardGame.Cards.Base;
using CardGame.Players;
using CardGame.Units.Base;

namespace Gameplay.Units
{
    public class HarmlessDragonEgg : Unit
    {
        public HarmlessDragonEgg(Card card, Player player, string instanceID = null) : base(card, player, instanceID)
        {
            _unitName = "Harmless Dragon Egg";
            _health = _maxHealth = 1;
            _damage = 0;
            _race = UnitRace.Dragon;

            //description: It's a harmless dragon egg, what could it possibly do?
            //Last Breath: Summon an angry dragon momma 6/6
            //cost: 3
            //max stacks: 2
        }
    }
}
