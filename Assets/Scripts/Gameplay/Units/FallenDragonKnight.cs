using CardGame.Units.Base;

namespace Gameplay.Units
{
    public class FallenDragonKnight : Unit
    {
        public FallenDragonKnight(string instanceID = null) : base(instanceID)
        {
            _unitName = "Fallen Dragon Knight";
            _health = _maxHealth = 4;
            _damage = 2;

            //Spawn: summon a 1/1 draconian squire
            //Provoke: Forces enemies to focus this unit.
            //max stack: 4
            //cost: 3
        }
    }
}
