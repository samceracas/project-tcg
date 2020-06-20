using CardGame.Units.Base;

namespace Gameplay.Units
{
    public class AngryDragonMomma : Unit
    {
        public AngryDragonMomma(string instanceID = null) : base(instanceID)
        {
            _unitName = "Angry Dragon Momma";
            _health = _maxHealth = 6;
            _damage = 6;

            //description: You're in for it now!
        }
    }
}
