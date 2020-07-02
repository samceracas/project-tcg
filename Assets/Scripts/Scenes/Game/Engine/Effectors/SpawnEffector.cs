using CardGame.Effectors.Base;
using CardGame.Players;
using CardGame.Units.Base;

namespace CardGame.Effectors
{
    class SpawnEffector : Effector
    {
        public SpawnEffector(Player player, Unit unit) : base()
        {
            _player = player;
            _target = unit;
            _id = "spawn_effector";
            _instanceID = Utils.Random.RandomString(15);
        }

        public override void Apply()
        {
            if (_player.UnitsOnField.Count >= _player.Game.Settings.MaxUnitsOnField)
            {
                _player.EventMoveError($"Cannot spawn {_target.UnitName}, battlefield is full.");
                return;
            }

            if (_player.Game.GameState == GameState.Ended)
            {
                _player.EventMoveError($"Game ended!");
                return;
            }
            
            _target.Spawn();
        }

        public override void Revert()
        {
            //this would undo the spawn without triggering unit_kill interceptors
            _player.KillUnit(_target);
        }
    }
}
