using CardGame;
using CardGame.Cards.Base;
using CardGame.Players;
using CardGame.Units.Base;
using Gameplay.Cards;

namespace Gameplay.Units.Classes
{
    public class DragonLordClass : Player, IPlayerClass
    {
        public DragonLordClass(string name, Game game) : base(name, game)
        {
            _health = _maxHealth = 30;
            _damage = 1;
            //a class which specializes in summoning dragons
            //buffs spawned dragons

            //tier 1:
            //  Dragons cost 1 less
            //  Dragonic spells cost 1 less, Dragons cost 2 less
            //  


            //add cards to deck
            for (int k = 0; k < 4; k++)
            {
                this.AddCardOnDeck(new HarmlessDragonEggCard(this));
            }

            //add cards to deck
            for (int k = 0; k < 4; k++)
            {
                this.AddCardOnDeck(new DragonLampreyCard(this));
            }

            //add cards to deck
            for (int k = 0; k < 4; k++)
            {
                this.AddCardOnDeck(new KoboldSpellCasterCard(this));
            }

            //add cards to deck
            for (int k = 0; k < 4; k++)
            {
                this.AddCardOnDeck(new KoboldArcherCard(this));
            }

            //add cards to deck
            for (int k = 0; k < 4; k++)
            {
                this.AddCardOnDeck(new DragonKnightEmperorCard(this));
            }

            //add cards to deck
            for (int k = 0; k < 4; k++)
            {
                this.AddCardOnDeck(new FallenDragonKnightCard(this));
            }


            //add cards to deck
            for (int k = 0; k < 4; k++)
            {
                this.AddCardOnDeck(new KoboldSpearmanCard(this));
            }


            this.AddCardOnDeck(new ImperialDragonSovereignCard(this));
        }

        public override void AddCardOnDeck(Card card)
        {
            if (card.Race == UnitRace.Dragon)
            {
                card.Cost -= 1;
            }
            base.AddCardOnDeck(card);
        }

        public string ClassName => "Dragon Lord";
        public int ClassTier => 1;
        public string ClassID => "dragon_lord_class";
    }
}
