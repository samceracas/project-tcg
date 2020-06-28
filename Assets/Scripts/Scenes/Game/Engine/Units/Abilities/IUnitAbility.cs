using CardGame.Units.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame.Units.Abilities
{
    interface IUnitAbility
    {
        string ID { get; }
        string InstanceID { get; }
        bool IsActive { get; }
        bool IsDuplicateAllowed { get; }
        void Listen(bool applyDirectly = false);
        void Revert();
        void AddToUnit(bool applyDirectly = false);
        void RemoveFromUnit();
        Unit Unit { get; }
    }
}
