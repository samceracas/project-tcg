using CardGame.Units.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame.Units.Abilities
{
    public class UnitAbility : IUnitAbility
    {

        protected string _id, _instanceID;
        protected bool _isActive, _isDuplicateAllowed = false;
        protected Unit _unit;

        public UnitAbility(Unit unit)
        {
            _unit = unit;
            _isActive = false;
            _isDuplicateAllowed = false;
            _instanceID = Utils.Random.RandomString(15);
        }

        public string ID => _id;
        public string InstanceID => _instanceID;
        public bool IsActive => _isActive;
        public bool IsDuplicateAllowed => _isDuplicateAllowed;
        public Unit Unit => _unit;

        public virtual void Revert()
        {

        }

        public virtual void Listen(bool applyDirectly = false)
        {
        }

        public void AddToUnit(bool applyDirectly = false)
        {
            _unit.AddAbility(this, applyDirectly);
        }

        public void RemoveFromUnit()
        {
            _unit.RemoveAbility(this);
        }
    }
}
