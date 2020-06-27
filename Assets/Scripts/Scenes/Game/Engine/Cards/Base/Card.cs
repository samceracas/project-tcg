using CardGame.Effectors;
using CardGame.Effectors.Base;
using CardGame.Players;
using CardGame.Units.Base;
using System;
using System.Linq;

namespace CardGame.Cards.Base
{
    public class Card : ICard
    {
        protected string _id, _name, _description;
        protected int _cost, _defaultCost, _targets;
        protected int _costModifiedState;
        protected Player _player;
        protected Effector _effector;
        protected CardType _cardType;
        protected int _attack;
        protected int _health;
        protected string _instanceID;
        protected Unit _unit;
        protected UnitRace _race;

        public Card(Player player, string instanceID = null)
        {
            _id = GetType().Name;
            _id = string.Concat(_id.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
            _player = player;
            _instanceID = instanceID == null ? Utils.Random.RandomString(15) : instanceID;
            _health = -1;
            _race = UnitRace.None;
            _costModifiedState = 0;
        }

        public string ID => _id;
        public string InstanceID { get => _instanceID; }

        public string Name => _name;

        public string Description => _description;

        public int Cost {
            get {
                return _cost;
            }
            set {
                _cost = value;
                _costModifiedState = _cost - _defaultCost;

                if (_costModifiedState != 0)
                {
                    //-1 means that the new cost is less than the default cost
                    //1 means that the new cost is greater than the default cost
                    _costModifiedState = Math.Abs(_costModifiedState) / _costModifiedState;
                }

                _player.EventCardCostUpdated(this);
            }
        }

        public int DefaultCost => _defaultCost;

        public int CostModifiedState => _costModifiedState;

        public Player Player { get => _player; set => _player = value; }

        public Effector Effector => _effector;

        public bool IsUnitCard { get => _cardType == CardType.Unit; }

        public CardType Type => _cardType;
        public UnitRace Race => _race;

        public int Attack { get => _attack; set => _attack = value; }
        public int Health { get => _attack; set => _attack = value; }
        public Unit Unit => _unit;

        public virtual void Apply()
        {
            //this will only trigger when the card is a unit card
            //only override when card is a spell card
            SpawnUnit();
        }

        protected virtual bool CanSpawnUnit()
        {
            return _player.UnitsOnField.Count < 7 && _player.Game.GameState == GameState.InProgress;
        }

        protected virtual void SpawnUnit()
        {
            if (_unit != null && _cardType == CardType.Unit && CanSpawnUnit())
            {
                SpawnEffector spawnEffector = new SpawnEffector(_unit);
                spawnEffector.Card = this;
                spawnEffector.Player = _player;
                _player.EffectorStack.ApplyEffector(spawnEffector);
            }
        }

        public virtual bool IsUsable()
        {
            return _player.Charges >= _cost && _player.IsMyTurn && _player.Game.GameState == GameState.InProgress;
        }
    }
}
