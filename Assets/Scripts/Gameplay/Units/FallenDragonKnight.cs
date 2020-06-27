using CardGame.Cards.Base;
using CardGame.Players;
using CardGame.Units.Base;

namespace Gameplay.Units
{
    public class FallenDragonKnight : Unit
    {
        public FallenDragonKnight(Card card, Player player, string instanceID = null) : base(card, player, instanceID)
        {
            _unitName = "Fallen Dragon Knight";
            _health = _maxHealth = 4;
            _damage = 2;
            _race = UnitRace.Dragon;

            //Spawn: summon a 1/1 draconian squire
            //Provoke: Forces enemies to focus this unit.
            //max stack: 4
            //cost: 3
        }
    }
}
