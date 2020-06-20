using CardGame.Cards.Base;
using CardGame.Effectors;
using CardGame.Players;

namespace CardGame.Units.Base
{
    public enum UnitState
    {
        GettingReady,
        Ready,
        Attacking,
        Dead
    }

    interface IUnit
    {
        UnitState State { get; }
        string ID { get; }
        string UnitName { get; }
        string InstanceID { get; }
        void ReceiveDamage(Unit dealer, int damage, EffectType damageSource);
        void Attack(Unit target, bool fromRetaliation = false);
        void Heal(Unit dealer, int amount);
        void AddCurrentHealth(int amount);
        void Spawn(bool triggerInterceptors = true);
        void Die(bool triggerInterceptors = true);
        void SetGettingReady();
        void SetReady();
        int Health { get; set; }
        int MaxHealth { get; set; }
        int Damage { get; set; }
        Player Owner { get; set; }
        Card Card { get; set; }
    }
}
