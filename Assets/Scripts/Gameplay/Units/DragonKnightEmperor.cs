using CardGame.Units.Base;

namespace Gameplay.Units
{
    public class DragonKnightEmperor : Unit
    {
        public DragonKnightEmperor(string instanceID = null) : base(instanceID)
        {
            _unitName = "Dragon Knight Emperor";
            _health = _maxHealth = 5;
            _damage = 5;

            //passive: increase dragons health and attack by 2/2
            //cost: 5
            //max stacks: 2
        }
    }
}
