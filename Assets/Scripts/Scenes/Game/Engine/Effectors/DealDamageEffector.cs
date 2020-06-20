using CardGame.Effectors.Base;

namespace CardGame.Effectors
{
    public enum EffectType
    {
        Attack,
        Heal,
        CriticalDamage,
        Magic,
        Burn,
        Poison,
        FrostBite,
        Death
    };

    class DealDamageEffector : Effector
    {

        private EffectType _effectType;

        public DealDamageEffector(int damage, EffectType damageSource) : base()
        {
            _id = "deal_damage_effector";
            _damage = damage;
            _effectType = damageSource;
        }

        public override void Apply()
        {
            _target.ReceiveDamage(_dealer, _damage, _effectType);
        }

        public override void Revert()
        {
            _target.AddCurrentHealth(_damage);
        }
    }
}
