namespace CardGame.Units.Base
{
    //todo
    interface IStatusEffect
    {
        string Name { get; }
        int RoundDuration { get; }
        void Tick();
    }
}
