using CardGame.Cards.Base;
using CardGame.Constants;
using CardGame.Effectors;
using CardGame.Players;
using CardGame.Units.Abilities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using UnityEngine;

namespace CardGame.Units.Base
{
    public class Unit : IUnit
    {

        protected string _instanceID;
        protected string _id;
        protected int _health, _maxHealth;
        protected int _damage = 1;
        protected int _accuracy = 100;
        protected Player _owner;
        protected UnitState _unitState;
        protected UnitRace _race;
        protected Card _card;
        protected string _unitName;
        protected List<UnitAbility> _abilities;
        protected List<Unit> _unitWhiteList;
        protected bool _unitWhiteListActive;

        public Action EventUnitReadyStateChanged = () => { };
        public Action EventAbilitiesUpdated = () => { };
        public Action<int, EffectType> EventUnitDamaged = (a, b) => { };
        public Action<int, EffectType> EventUnitHealed = (a, b) => { };

        public Unit(Card card, Player player, string instanceID = null)
        {
            _id = GetType().Name;
            _id = string.Concat(_id.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
            _instanceID = instanceID == null ? Utils.Random.RandomString(15) : instanceID;
            _unitState = UnitState.Idle;
            _race = UnitRace.Human;
            _owner = player;
            _card = card;
            _abilities = new List<UnitAbility>();
            _unitWhiteList = new List<Unit>();
            _unitWhiteListActive = false;

            ReadyEvents();
        }

        public string ID => _id;
        public string InstanceID => _instanceID;
        public string UnitName => _unitName;
        public int Health { get => _health; set => _health = value; }
        public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }
        public Player Owner { get => _owner; set => _owner = value; }
        public int Damage { get => _damage; set => _damage = value; }
        public UnitState State => _unitState;
        public UnitRace Race => _race;
        public Card Card { get => _card; set => _card = value; }
        public UnitAbility[] Abilities { get => _abilities.ToArray(); }
        public bool AttackWhiteListActive { get => _unitWhiteListActive; set => _unitWhiteListActive = value; }

        public void ReceiveDamage(Unit dealer, int damage, EffectType effectType, bool checkDeath = false)
        {
            _health -= damage;

            if (!_owner.IsSimulated)
            {
                EventUnitDamaged(damage, effectType);
            }

            if (checkDeath)
            {
                CheckDeath(dealer);
            }
        }

        public void AddCurrentHealth(int amount)
        {
            _health += amount;

            if (_health >= _maxHealth)
            {
                _health = _maxHealth;
            }

            if (!_owner.IsSimulated)
            {
                EventUnitHealed(amount, EffectType.Heal);
            }
        }

        public virtual void Attack(Unit target, bool checkDeath = false, bool retaliate = false)
        {
            _unitState = UnitState.Attacking;


            dynamic parameters = new ExpandoObject();
            parameters.dealer = this;
            parameters.target = target;

            Action wrappedMethod = () =>
            {
                DealDamageEffector effector = new DealDamageEffector(_damage, EffectType.Attack, checkDeath);
                effector.Target = target;
                effector.Dealer = this;
                effector.Player = _owner;

                _owner.EffectorStack.ApplyEffector(effector);

                if (retaliate)
                {
                    target.Attack(this, true, false);
                }
            };
            _owner.Game.InterceptorService.RunInterceptor(InterceptorEvents.UnitAttack, wrappedMethod, parameters);
        }

        public virtual void Heal(Unit dealer, int amount)
        {
            if (_unitState == UnitState.Dead)
            {
                _unitState = UnitState.GettingReady;
            }
            dynamic parameters = new ExpandoObject();

            parameters.dealer = dealer;
            parameters.target = this;


            Action wrappedMethod = () =>
            {
                HealEffector effector = new HealEffector(amount);
                effector.Target = this;
                effector.Dealer = dealer;
                effector.Player = _owner;

                _owner.EffectorStack.ApplyEffector(effector);
            };

            _owner.Game.InterceptorService.RunInterceptor(InterceptorEvents.UnitHeal, wrappedMethod, parameters);
        }

        public void Die(bool triggerInterceptors = true)
        {
            dynamic parameters = new ExpandoObject();
            parameters.unit = this;
            Action wrappedMethod = () =>
            {
                _unitState = UnitState.Dead;
                _owner.KillUnit(this);
            };

            if (triggerInterceptors)
            {
                _owner.Game.InterceptorService.RunInterceptor(InterceptorEvents.UnitKill, wrappedMethod, parameters);
            }
            else
            {
                wrappedMethod();
            }
        }

        public void Spawn(bool triggerInterceptors = true)
        {
            dynamic parameters = new ExpandoObject();
            parameters.unit = this;

            Action wrappedMethod = () =>
            {
                SetGettingReady();
                _owner.SpawnUnit(this);
            };



            if (triggerInterceptors)
            {
                _owner.Game.InterceptorService.RunInterceptor(InterceptorEvents.UnitSpawn, wrappedMethod, parameters);
            }
            else
            {
                wrappedMethod();
            }
        }

        public void SetReady()
        {
            _unitState = UnitState.Ready;
            EventUnitReadyStateChanged();
        }

        public void SetGettingReady()
        {
            _unitState = UnitState.GettingReady;
            EventUnitReadyStateChanged();
        }

        public virtual void ReadyEvents()
        {
        }

        public virtual void ClearEvents()
        {
            ClearAbilities();

            EventUnitHealed = (a, b) => { };
            EventUnitDamaged = (a, b) => { };
            EventUnitReadyStateChanged = () => { };
        }

        public void CheckDeath(Unit dealer)
        {
            if (_health <= 0)
            {
                _unitState = UnitState.Dead;

                DeathEffector deathEffector = new DeathEffector();
                deathEffector.Dealer = dealer;
                deathEffector.Target = this;
                deathEffector.Player = _owner;

                //apply effector
                _owner.EffectorStack.ApplyEffector(deathEffector);
            }
        }

        public void AddAbility(UnitAbility ability, bool applyDirectly = false)
        {
            if (!HasAbility(ability.ID))
            {
                ability.Listen(applyDirectly);
                _abilities.Add(ability);
                EventAbilitiesUpdated();
            }
        }

        public void RemoveAbility(string id)
        {
            if (HasAbility(id))
            {
                UnitAbility ability = _abilities.Find((a) => a.ID.Equals(id));
                RemoveAbility(ability);
            }
        }

        public void RemoveAbility(UnitAbility ability)
        {
            if (HasAbility(ability))
            {
                ability.Revert();
                _abilities.Remove(ability);
                EventAbilitiesUpdated();
            }
        }

        public bool HasAbility(string id)
        {
            UnitAbility ability = _abilities.Find((a) => a.ID.Equals(id));
            return ability != null;
        }

        public bool HasAbility(UnitAbility ability)
        {
            return _abilities.Contains(ability);
        }

        public void ClearAbilities()
        {
            foreach (UnitAbility ability in _abilities)
            {
                ability.Revert();
            }

            _abilities = new List<UnitAbility>();
        }

        public bool CanAttackUnit(Unit unit)
        {
            return !_unitWhiteListActive || (_unitWhiteList.Contains(unit) && _unitWhiteListActive);
        }

        public void AddUnitToAttackWhiteList(Unit unit)
        {
            if (!_unitWhiteList.Contains(unit))
            {
                _unitWhiteList.Add(unit);
            }
        }

        public void RemoveUnitFromAttackWhiteList(Unit unit)
        {
            if (_unitWhiteList.Contains(unit))
            {
                _unitWhiteList.Remove(unit);
            }
        }

        public void ClearAttackWhiteList()
        {
            _unitWhiteList = new List<Unit>();
        }
    }
}
