using CardGame.Units.Base;

namespace Gameplay.Units
{
    public class ImperialDragonSovereign : Unit
    {
        public ImperialDragonSovereign(string instanceID = null) : base(instanceID)
        {
            //generate a name here: https://donjon.bin.sh/fantasy/name
            _unitName = "Imperial Dragon Sovereign";
            _health = _maxHealth = 11;
            _damage = 11;

            //cost: 10
            //active: Set all enemy dragons to 1/1
            //max stacks: 1
        }
    }
}
