using CardGame.Units.Base;

namespace CardGame.Units
{
    class FlameDrake : Unit
    {
        public FlameDrake(string instanceID = null) : base(instanceID)
        {
            _id = "flame_drake";
            _unitName = "Flame Drake";
            _health = _maxHealth = 1;
            _damage = 1;
        }
    }
}
