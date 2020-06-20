using CardGame.Units.Base;

namespace Gameplay.Units
{
    public class HarmlessDragonEgg : Unit
    {
        public HarmlessDragonEgg(string instanceID = null) : base(instanceID)
        {
            _unitName = "Harmless Dragon Egg";
            _health = _maxHealth = 1;
            _damage = 0;

            //description: It's a harmless dragon egg, what could it possibly do?
            //Last Breath: Summon an angry dragon momma 6/6
            //cost: 3
            //max stacks: 2
        }
    }
}
