using CardGame.Cards.Base;
using CardGame.Effectors.Base;
using CardGame.Units.Base;
using System.Collections.Generic;

namespace CardGame.Players
{
    interface IPlayer
    {
        string Name { get; }
        int Charges { get; }
        bool IsSimulated { get; set; }
        bool IsMyTurn { get; }
        Dictionary<string, Unit> UnitsOnField { get; }
        Dictionary<string, Unit> UnitsDied { get; }
        List<Card> CardsOnDeck { get; }
        List<Card> CardsOnHand { get; }
        List<Card> CardsOnGraveyard { get; }
        EffectorStack EffectorStack { get; }
        UnitSelector UnitSelector { get; }
        Game Game { get; }
        Player Opponent { get; }
        void Ready();
        void SpawnUnit(Unit unit);
        void KillUnit(Unit unit);
        void AddCardToHand(Card card, bool isDrawn = true);
        void AddCardOnDeck(Card card);
        void DrawCards(int count);
        void StartTurn();
        void EndTurn(bool gameOver = false);
        void UseCard(int cardIndex);
        void CommandAttack(string instanceID);
        void ResetTurn();
    }
}
