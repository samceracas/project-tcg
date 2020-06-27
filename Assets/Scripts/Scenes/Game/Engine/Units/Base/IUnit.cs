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

    public enum UnitRace
    {
        Dragon,
        Human,
        Elemental,
        Undead,
        Mech,
        Goblin,
        Beast,
        None
    }

    interface IUnit
    {
        UnitRace Race { get; }
        UnitState State { get; }
        string ID { get; }
        string UnitName { get; }
        string InstanceID { get; }
        void ReceiveDamage(Unit dealer, int damage, EffectType damageSource, bool checkDeath);
        void Attack(Unit target, bool checkDeath, bool retaliate);
        void Heal(Unit dealer, int amount);
        void AddCurrentHealth(int amount);
        void Spawn(bool triggerInterceptors = true);
        void Die(bool triggerInterceptors = true);
        void SetGettingReady();
        void SetReady();
        void ReadyEvents();
        void ClearEvents();
        void CheckDeath(Unit unit);
        int Health { get; set; }
        int MaxHealth { get; set; }
        int Damage { get; set; }
        Player Owner { get; set; }
        Card Card { get; set; }
    }
}
