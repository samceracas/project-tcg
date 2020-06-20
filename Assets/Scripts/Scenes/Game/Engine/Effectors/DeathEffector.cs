using CardGame.Effectors.Base;
using UnityEngine;

namespace CardGame.Effectors
{
    class DeathEffector : Effector
    {

        public DeathEffector() : base()
        {
            _id = "death_effector";
        }

        public override void Apply()
        {
            _target.Die();
        }

        public override void Revert()
        {
            _target.Spawn(false);
        }
    }
}
