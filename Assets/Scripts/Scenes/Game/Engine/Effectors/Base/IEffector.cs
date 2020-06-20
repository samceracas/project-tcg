using CardGame.Cards.Base;
using CardGame.Players;
using CardGame.Units.Base;

namespace CardGame.Effectors.Base
{
    interface IEffector
    {
        string InstanceID { get; }
        string ID { get; }
        Card Card { get; set; }
        Unit Dealer { get; set; }
        Unit Target { get; set; }
        Player Player { get; set; }
        void Apply();
        void Revert();
    }
}
