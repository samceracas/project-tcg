using CardGame.Cards.Base;
using CardGame.Players;
using CardGame.Units.Base;

namespace CardGame.Effectors.Base
{
    public class Effector : IEffector
    {
        protected string _id;
        protected int _damage;
        protected Unit _dealer;
        protected Unit _target;
        protected Player _player;
        protected Card _card;
        protected string _instanceID;

        public Effector()
        {
            _instanceID = Utils.Random.RandomString(15);
        }

        public string InstanceID => _instanceID;
        public string ID => _id;
        public Card Card { get => _card; set => _card = value; }
        public Player Player { get => _player; set => _player = value; }
        public Unit Dealer { get => _dealer; set => _dealer = value; }
        public Unit Target { get => _target; set => _target = value; }

        public virtual void Apply()
        {
        }

        public virtual void Revert()
        {
        }
    }
}
