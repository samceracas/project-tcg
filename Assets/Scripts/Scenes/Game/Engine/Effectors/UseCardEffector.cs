using CardGame.Cards.Base;
using CardGame.Effectors.Base;

namespace CardGame.Effectors
{
    class UseCardEffector : Effector
    {

        protected Card _cardToUse;

        public UseCardEffector(Card cardToUse) : base()
        {
            _id = "use_card_effector";
            _cardToUse = cardToUse;
        }

        public override void Apply()
        {
            _player.CardsOnHand.Remove(_cardToUse);
            _player.CardsOnGraveyard.Add(_cardToUse);
        }

        public override void Revert()
        {
            _player.CardsOnGraveyard.Remove(_cardToUse);
            _player.CardsOnHand.Add(_cardToUse);
        }
    }
}
