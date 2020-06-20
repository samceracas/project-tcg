using CardGame.Units.Base;

namespace Gameplay.Units
{
    public class KoboldSpellCaster : Unit
    {
        public KoboldSpellCaster(string instanceID = null) : base(instanceID)
        {
            _unitName = "Kobold Spell Caster";
            _health = _maxHealth = 1;
            _damage = 2;

            //Draw a random spell card
            //max stacks: 4
            //cost: 1
        }
    }
}
