using CardGame.Units.Base;

namespace Gameplay.Units
{
    class DraconianSquire : Unit
    {
        public DraconianSquire(string instanceID = null) : base(instanceID)
        {
            _unitName = "Draconic Squire";
            _health = _maxHealth = 1;
            _damage = 1;

            //description: Every knight needs a squire!
            //max stacks: 4
        }
    }
}
