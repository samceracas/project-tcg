using CardGame.Effectors;
using CardGame.Effectors.Base;
using CardGame.Players;
using CardGame.Units.Base;
using System.Linq;

namespace CardGame.Cards.Base
{
    public class Card : ICard
    {
        protected string _id, _name, _description;
        protected int _cost, _targets;
        protected Player _player;
        protected Effector _effector;
        protected CardType _cardType;
        protected int _attack;
        protected int _health;
        protected string _instanceID;
        protected Unit _unit;

        public Card(Player player, string instanceID = null)
        {
            _id = GetType().Name;
            _id = string.Concat(_id.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
            _player = player;
            _instanceID = instanceID == null ? Utils.Random.RandomString(15) : instanceID;
            _health = -1;
        }

        public string ID => _id;
        public string InstanceID { get => _instanceID; }

        public string Name => _name;

        public string Description => _description;

        public int Cost => _cost;

        public Player Player { get => _player; set => _player = value; }

        public Effector Effector => _effector;


        public CardType Type => _cardType;

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
            return _player.UnitsOnField.Count < 7;
        }

        protected virtual void SpawnUnit()
        {
            if (_cardType == CardType.Unit && CanSpawnUnit() && _unit != null)
            {
                SpawnEffector spawnEffector = new SpawnEffector(_unit);
                spawnEffector.Card = this;
                spawnEffector.Player = _player;
                _player.EffectorStack.ApplyEffector(spawnEffector);
            }
        }

        public virtual bool IsUsable()
        {
            return _player.Charges >= _cost && _player.IsMyTurn;
        }
    }
}
