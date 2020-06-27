
using CardGame.Cards.Base;
using CardGame.Constants;
using CardGame.Units.Base;
using CardGame.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace CardGame.Players
{
    public class AIPlayer : Player
    {
        protected float _moveScore;
        protected Queue<string> _moveQueue;

        public AIPlayer(Game game) : base("ai:steve", game)
        {
            _name = "ai:steve";
            _moveScore = 0;
            _moveQueue = new Queue<string>();

            ReadyEvaluators();
        }

        public float MoveScore => _moveScore;
        public Queue<string> MoveQueue => _moveQueue;


        public override void StartTurn()
        {
            //todo move this to AI player
            base.StartTurn();

            _moveScore = 0f;
            _moveQueue = new Queue<string>();

            Queue<string> evaluatedMove = new Queue<string>();
            float currentHighScore = 0;
            Randomizer randomizer = new Randomizer();
            int possibleMoves = GetPlayableCards(randomizer).Count + GetUnitAttackOrder(randomizer).Count;
            for (int i = 0; i < possibleMoves; i++)
            {
                using (Game copy = _gameInstance.ToCopy())
                {
                    AIPlayer me = (AIPlayer)copy.GetPlayer(_instanceID);
                    Player opponent = (Player)copy.GetOpponent(me);
                    
                    opponent.IsSimulated = true;
                    me.IsSimulated = true;

                    me.SimulateRound();
                    if (me.MoveScore >= currentHighScore)
                    {
                        evaluatedMove = me.MoveQueue;
                        currentHighScore = me.MoveScore;
                    }
                }
            }

            if (!IsSimulated)
            {
                EventMoveQueueReady.Invoke(evaluatedMove);
            }
        }

        public void SimulateRound()
        {
            Randomizer randomizer = new Randomizer();
            List<Card> playableCards = GetPlayableCards(randomizer);
            if (playableCards.Count > 0)
            {
                foreach (Card playableCard in playableCards)
                {
                    if (_gameInstance.GameState == GameState.Ended)
                    {
                        Debug.Log("Game ended, stopping use card phase simulation.");
                        break;
                    }
                    if (playableCard is IAimable)
                    {
                        Debug.Log($"{playableCard.Name} is aimable! Todo logic here!");
                    }
                    else
                    {
                        _moveQueue.Enqueue("use_card:" + _cardsOnHand.IndexOf(playableCard));
                        UseCard(_cardsOnHand.IndexOf(playableCard));
                    }
                }
            }

            if (_unitsOnField.Count > 0)
            {
                List<string> unitAttackOrder = GetUnitAttackOrder(randomizer);
                List<string> targets = GetPotentialTargets();

                Player opponent = _gameInstance.GetOpponent(this);


                int targetIndex = 0;

                for (int i = 0; i < unitAttackOrder.Count; i++)
                {
                    if (_gameInstance.GameState == GameState.Ended)
                    {
                        Debug.Log("Game ended, stopping attack phase simulation.");
                        break;
                    }
                    string myUnitInstanceID = unitAttackOrder[i];
                    CommandAttack(myUnitInstanceID);

                    List<string> strongUnits = targets.FindAll(entry =>
                    {
                        if (!opponent.UnitsOnField.ContainsKey(entry)) return false;
                        return opponent.UnitsOnField[entry].Damage >= 3;
                    });

                    //default behavior: the higher the unit count on the enemy field, the less chance the AI attacks the player
                    float attackPlayerScore = strongUnits.Count > 0 ? 0 : ((1f - ((opponent.UnitsOnField.Count * 1f) / 6f)) * 100f);
                    foreach (string strongUnit in strongUnits)
                    {
                        float unitDamage = opponent.UnitsOnField[strongUnit].Damage;
                        attackPlayerScore += (1f - (unitDamage / 60f)) * 50f;
                    }

                    if (opponent.UnitsOnField.Count <= 1) attackPlayerScore = 100f;

                    int roll = randomizer.RandomInt(0, 100);

                    if (roll <= attackPlayerScore)
                    {
                        //Debug.Log("Attacking player, attack player score: " + attackPlayerScore + ", roll: " + roll);
                        _unitSelector.Select(opponent);
                        _moveQueue.Enqueue($"attack:{myUnitInstanceID}:opponent");
                    }
                    else
                    {
                        string targetKey = targets[targetIndex];
                        if (opponent.UnitsOnField.ContainsKey(targetKey))
                        {
                            //Debug.Log("Attacking unit, attack player score: " + attackPlayerScore + ", roll: " + roll);
                            _unitSelector.Select(opponent.UnitsOnField[targetKey]);
                            _moveQueue.Enqueue($"attack:{myUnitInstanceID}:{targetKey}");
                        }
                    }

                    targetIndex = targetIndex + 1 >= targets.Count ? 0 : ++targetIndex;
                }
            }
        }

        protected virtual List<string> GetUnitAttackOrder(Randomizer randomizer)
        {
            List<string> targets = new List<string>();
            Dictionary<string, Unit> units = new Dictionary<string, Unit>(_unitsOnField);
            var unitList = units.ToList();
            unitList.Shuffle(randomizer);

            foreach (KeyValuePair<string, Unit> keyValuePair in unitList)
            {
                if (keyValuePair.Value.State == UnitState.Ready)
                {
                    targets.Add(keyValuePair.Key);
                }
            }

            return targets;
        }

        protected virtual List<string> GetPotentialTargets()
        {
            Player opponentPlayer = _gameInstance.GetOpponent(this);
            List<string> targets = new List<string>();
            Dictionary<string, Unit> units = new Dictionary<string, Unit>(opponentPlayer.UnitsOnField);
            var unitList = units.ToList();

            //sort units by health
            unitList.Sort((a, b) => a.Value.Health.CompareTo(b.Value.Health));

            foreach (KeyValuePair<string, Unit> keyValuePair in unitList)
            {
                targets.Add(keyValuePair.Key);
            }

            return targets;
        }

        protected virtual List<Card> GetPlayableCards(Randomizer randomizer)
        {
            List<Card> cardsToPlay = new List<Card>();
            List<Card> cards = new List<Card>(_cardsOnHand);
            cards.Shuffle(randomizer);

            int chargesLeft = _charges;

            foreach (Card card in cards)
            {
                if (chargesLeft <= 0) break;

                if (card.Cost <= chargesLeft)
                {
                    cardsToPlay.Add(card);
                    chargesLeft -= card.Cost;
                }
            }

            return cardsToPlay;
        }

        protected virtual void ReadyEvaluators()
        {
            //add move scores here based on the effectors that are run
            _gameInstance.InterceptorService.RegisterInterceptor(InterceptorEvents.UnitKill, new Events.Interceptor()
            {
                Priority = -2,
                After = (state) =>
                {
                    Unit unit = state.Arguments.unit;

                    if (unit is Player)
                    {
                        _moveScore = int.MaxValue;
                    }
                    else
                    {
                        _moveScore += unit.Owner.UnitsDied.Count;
                    }

                    return state;
                }
            });

            _gameInstance.InterceptorService.RegisterInterceptor(InterceptorEvents.UnitAttack, new Events.Interceptor()
            {
                After = (state) =>
                {
                    Unit dealer = state.Arguments.dealer;
                    Unit target = state.Arguments.target;
                    float targetHealth = target.Health;
                    float targetMaxHealth = target.MaxHealth;

                    if (target is Player)
                    {
                        Player playerTarget = (Player)target;
                        _moveScore += (1f - (playerTarget.UnitsOnField.Count / 6f)) * 5f;
                    }

                    _moveScore += (5f * (1f - (targetHealth / targetMaxHealth)));

                    return state;
                }
            });
        }
    }
}
