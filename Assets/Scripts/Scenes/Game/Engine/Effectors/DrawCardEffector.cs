using CardGame.Cards.Base;
using CardGame.Effectors.Base;
using CardGame.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Effectors
{
    class DrawCardEffector : Effector
    {

        protected int _count;
        protected List<Card> _drawnCards;

        public DrawCardEffector(int count) : base()
        {
            _id = "draw_card_effector";
            _count = count;
        }

        public override void Apply()
        {
            if (_player == null)
            {
                Debug.LogError("Draw cards effector could not be applied, _player is null.");
                return;
            }

            if (_drawnCards == null)
            {
                _drawnCards = new List<Card>();

                //shuffle deck
                _player.CardsOnDeck.Shuffle(_player.Game.Randomizer);

                if (_player.CardsOnDeck.Count <= 0)
                {
                    Debug.Log("No cards to draw!");
                    return;
                }

                for (int i = 0; i < _count; i++)
                {
                    _drawnCards.Add(_player.CardsOnDeck[i]);
                }
            }

            for (int i = 0; i < _drawnCards.Count; i++)
            {
                _player.AddCardToHand(_drawnCards[i], true);
            }

            _player.CardsOnDeck.RemoveAll((item) => _drawnCards.Contains(item));

        }

        public override void Revert()
        {
            if (_player == null || _drawnCards == null) return;

            _player.CardsOnHand.RemoveAll((item) => _drawnCards.Contains(item));
        }
    }
}
