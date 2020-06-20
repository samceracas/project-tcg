using CardGame.Cards.Base;
using CardGame.Effectors;
using CardGame.Players;
using CardGame.Units;

namespace CardGame.Cards
{
    class IceDrakeCard : Card
    {
        private IceDrake _iceDrake;

        public IceDrakeCard(Player player, string cardUID = null, string unitUID = null) : base(player, cardUID)
        {
            _cost = 1;
            _name = "Ice Drake";
            _description = "";
            _cardType = CardType.Unit;
            _iceDrake = new IceDrake(unitUID);
            _health = _iceDrake.Health;
            _attack = _iceDrake.Damage;
        }

        public override void Apply()
        {
            SpawnEffector spawnEffector = new SpawnEffector(_iceDrake);
            spawnEffector.Card = this;
            spawnEffector.Player = _player;
            _player.EffectorStack.ApplyEffector(spawnEffector);
        }
    }
}
