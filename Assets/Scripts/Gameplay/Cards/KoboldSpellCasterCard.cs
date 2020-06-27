using CardGame.Cards.Base;
using CardGame.Players;
using CardGame.Units.Base;
using Gameplay.Units;

namespace Gameplay.Cards
{
    public class KoboldSpellCasterCard : Card
    {
        public KoboldSpellCasterCard(Player player, string instanceID = null) : base(player, instanceID)
        {
            _cost = _defaultCost = 1;
            _name = "Kobold Spell Caster";
            _description = "";
            _cardType = CardType.Unit;
            _unit = new KoboldSpellCaster(instanceID);
            _health = _unit.Health;
            _attack = _unit.Damage;
            _race = UnitRace.Dragon;
        }
    }
}
