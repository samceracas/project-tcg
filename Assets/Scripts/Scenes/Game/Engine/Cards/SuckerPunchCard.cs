
using CardGame.Cards.Base;
using CardGame.Effectors;
using CardGame.Players;
using CardGame.Units.Base;

namespace CardGame.Cards
{
    class SuckerPunchCard : Card, IAimable
    {

        public SuckerPunchCard(Player player) : base(player)
        {
            _cost = 0;
            _name = "Sucker Punch!";
            _description = "Deal 1 damage to a target";

            _effector = new DealDamageEffector(1, EffectType.Magic);
            _effector.Dealer = _player;
            _effector.Card = this;
            _effector.Player = _player;
            _targets = 1;
            _cardType = CardType.Spell;
        }

        public int Targets => _targets;

        public void Apply(Unit target)
        {
            _effector.Target = target;
            _player.EffectorStack.ApplyEffector(_effector);
        }

        public bool ValidateTarget(Unit target)
        {
            return true;
        }
    }
}
