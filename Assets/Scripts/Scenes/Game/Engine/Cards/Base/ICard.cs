using CardGame.Effectors.Base;
using CardGame.Players;
using CardGame.Units.Base;

namespace CardGame.Cards.Base
{

    public enum CardType
    {
        Spell,
        Unit
    }

    interface ICard
    {
        string InstanceID { get; }
        string ID { get; }
        string Name { get; }
        string Description { get; }
        int Cost { get; set; }
        int DefaultCost { get; }
        CardType Type { get; }
        UnitRace Race { get; }
        
        bool IsUnitCard { get; }

        int CostModifiedState { get; }

        int Attack { get; set; }
        int Health { get; set; }
        Unit Unit { get; }

        Player Player { get; set; }
        Effector Effector { get; }

        void Apply();
        bool IsUsable();
    }
}
