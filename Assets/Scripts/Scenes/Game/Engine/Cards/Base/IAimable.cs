using CardGame.Units.Base;

namespace CardGame.Cards.Base
{
    interface IAimable
    {
        int Targets { get; }
        void Apply(Unit target);
        bool ValidateTarget(Unit target);
    }
}
