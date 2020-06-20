namespace CardGame.Players
{
    public interface IPlayerClass
    {
        string ClassName { get; }
        string ClassID { get; }
        int ClassTier { get; }

        //todo add active abilities
    }
}
