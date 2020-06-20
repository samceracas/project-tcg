using CardGame.Effectors.Base;
using CardGame.Units.Base;

namespace CardGame.Effectors
{
    class SpawnEffector : Effector
    {
        public SpawnEffector(Unit unit) : base()
        {
            _target = unit;
            _id = "spawn_effector";
            _instanceID = Utils.Random.RandomString(15);
        }

        public override void Apply()
        {
            if (_player.UnitsOnField.Count >= 7)
            {
                _player.MoveError($"Cannot spawn {_target.UnitName}, battlefield is full.");
                return;
            }

            _target.Owner = _player;
            _target.Card = _card;
            _target.Spawn();
        }

        public override void Revert()
        {
            //this would undo the spawn without triggering unit_kill interceptors
            _player.KillUnit(_target);
        }
    }
}
