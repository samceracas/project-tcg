using CardGame.Units.Base;

namespace CardGame.Units
{
    class IceDrake : Unit
    {
        public IceDrake(string instanceID) : base(instanceID)
        {
            _id = "ice_drake";
            _unitName = "Ice Drake";
            _health = _maxHealth = 1;
            _damage = 1;
        }
    }
}
