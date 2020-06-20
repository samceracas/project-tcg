using CardGame.Units.Base;

namespace Gameplay.Units
{
    public class KoboldArcher : Unit
    {
        public KoboldArcher(string instanceID = null) : base(instanceID)
        {
            _unitName = "Kobold Archer";
            _health = _maxHealth = 3;
            _damage = 2;

            //Nimble: can attack twice in 1 turn
            //max stacks: 4
            //cost: 2
        }
    }
}
