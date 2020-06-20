using CardGame.Cards.Base;
using CardGame.Effectors;
using CardGame.Players;
using CardGame.Units;

namespace CardGame.Cards
{
    class TinyIceDragonCard : Card
    {
        private FlameDrake _flameDrake;

        public TinyIceDragonCard(Player player) : base(player)
        {
            _cost = 1;
            _name = "Flame Drake";
            _description = "";
            _cardType = CardType.Unit;
            _flameDrake = new FlameDrake();
            _health = _flameDrake.Health;
            _attack = _flameDrake.Damage;
        }

        public override void Apply()
        {
            SpawnEffector spawnEffector = new SpawnEffector(_flameDrake);
            spawnEffector.Card = this;
            spawnEffector.Player = _player;
            _player.EffectorStack.ApplyEffector(spawnEffector);
        }
    }
}
