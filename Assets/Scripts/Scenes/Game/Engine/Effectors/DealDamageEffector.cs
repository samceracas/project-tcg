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
        private bool _checkDeath;

        public DealDamageEffector(int damage, EffectType damageSource, bool checkDeath) : base()
        {
            _id = "deal_damage_effector";
            _damage = damage;
            _effectType = damageSource;
            _checkDeath = checkDeath;
        }

        public override void Apply()
        {
            _target.ReceiveDamage(_dealer, _damage, _effectType, _checkDeath);
        }

        public override void Revert()
        {
            _target.AddCurrentHealth(_damage);
        }
    }
}
