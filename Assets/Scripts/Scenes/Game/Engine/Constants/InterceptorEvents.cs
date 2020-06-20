namespace CardGame.Constants
{
    class InterceptorEvents
    {
        public const string UnitSpawn = "unit_spawn";
        public const string UnitKill = "unit_kill";
        public const string UnitAttack = "unit_attack";
        public const string UnitHeal = "unit_heal";

        public const string AddCardToHand = "add_card_to_hand";
        public const string CardUse = "card_use";
        public const string DrawCard = "draw_card";

        public const string EndTurn = "end_turn";
        public const string StartTurn = "start_turn";
    }
}
