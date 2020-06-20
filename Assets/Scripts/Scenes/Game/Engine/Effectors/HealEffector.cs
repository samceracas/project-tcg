using CardGame.Effectors.Base;

namespace CardGame.Effectors
{
    class HealEffector : Effector
    {
        protected int _amount;

        public HealEffector(int amount) : base()
        {
            _amount = amount;
            _id = "heal_effector";
        }

        public override void Apply()
        {
            _target.AddCurrentHealth(_amount);
        }

        public override void Revert()
        {
            _target.ReceiveDamage(_dealer, _amount, EffectType.Heal);
        }
    }
}
