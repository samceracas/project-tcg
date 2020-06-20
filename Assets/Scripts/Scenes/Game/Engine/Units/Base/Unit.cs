using CardGame.Cards.Base;
using CardGame.Constants;
using CardGame.Effectors;
using CardGame.Players;
using System;
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
        protected Card _card;
        protected string _unitName;

        public Unit(string instanceID = null)
        {
            _id = GetType().Name;
            _id = string.Concat(_id.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
            _instanceID = instanceID == null ? Utils.Random.RandomString(15) : instanceID;
            _unitState = UnitState.GettingReady;
        }

        public string ID => _id;
        public string InstanceID => _instanceID;
        public string UnitName => _unitName;
        public int Health { get => _health; set => _health = value; }
        public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }
        public Player Owner { get => _owner; set => _owner = value; }
        public int Damage { get => _damage; set => _damage = value; }
        public UnitState State => _unitState;
        public Card Card { get => _card; set => _card = value; }

        public void ReceiveDamage(Unit dealer, int damage, EffectType damageSource)
        {

            _health -= damage;

            if (!_owner.IsSimulated)
            {
                _owner.UnitDamaged(this, damage, damageSource);
            }

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

        public void AddCurrentHealth(int amount)
        {
            _health += amount;

            if (_health >= _maxHealth)
            {
                _health = _maxHealth;
            }
        }

        public virtual void Attack(Unit target, bool fromRetaliation = false)
        {
            if ((_unitState == UnitState.Dead && !fromRetaliation) || (_unitState == UnitState.GettingReady && !fromRetaliation)) return;


            if ((_unitState == UnitState.Dead && fromRetaliation))
            {
                //stay dead
                _unitState = UnitState.Dead;
            }
            else
            {
                _unitState = UnitState.Attacking;
            }


            dynamic parameters = new ExpandoObject();
            parameters.dealer = this;
            parameters.target = target;

            Action wrappedMethod = () =>
            {
                DealDamageEffector effector = new DealDamageEffector(_damage, EffectType.Attack);
                effector.Target = target;
                effector.Dealer = this;
                effector.Player = _owner;

                _owner.EffectorStack.ApplyEffector(effector);
            };

            _owner.Game.InterceptorService.RunInterceptor(InterceptorEvents.UnitAttack, wrappedMethod, parameters);
        }

        public virtual void Heal(Unit dealer, int amount)
        {
            if (_unitState == UnitState.Dead) return;
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
            _owner.UnitReadyStateChanged(this);
        }

        public void SetGettingReady()
        {
            _unitState = UnitState.GettingReady;
            _owner.UnitReadyStateChanged(this);
        }
    }
}
