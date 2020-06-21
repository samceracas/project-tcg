using CardGame.Units.Base;

namespace Gameplay.Units
{
    public class DragonLamprey : Unit
    {
        public DragonLamprey(string instanceID = null) : base(instanceID)
        {
            _unitName = "Dragon Lamprey";
            _health = _maxHealth = 1;
            _damage = 2;
            _race = UnitRace.Dragon;

            //max stacks: 4
            //give 1 charge to the player
            //cost: 1
        }
    }
}
