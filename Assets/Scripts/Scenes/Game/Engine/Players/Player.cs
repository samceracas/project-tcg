using CardGame.Cards.Base;
using CardGame.Constants;
using CardGame.Effectors;
using CardGame.Effectors.Base;
using CardGame.Units.Base;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace CardGame.Players
{
    public enum PlayerState
    {
        Idle,
        Ready
    }

    public class Player : Unit, IPlayer
    {

        protected string _name;

        protected List<Card> _cardsOnDeck,
            _cardsOnHand;

        protected Dictionary<string, Unit> _unitsOnField, _unitsDied;
        protected List<Card> _cardsOnGraveyard;
        protected EffectorStack _effectorStack;

        protected UnitSelector _unitSelector;

        protected Game _gameInstance;
        protected CancellationTokenSource tokenSource;

        protected int _startTurnEffectorIndex;
        protected int _startTurnCount;
        protected int _charges;

        protected bool _isSimulated;

        protected PlayerState _playerState;

        #region Events/Requests
        public Action<Card> RequestUseCard;
        public Action<Unit, Unit> RequestCommandAttack;

        public Action<Card, bool> CardAddedToHand;

        public Action<Unit> UnitSpawn;
        public Action<Unit> UnitKill;
        public Action<Unit, int, EffectType> UnitDamaged;
        public Action<Unit> UnitReadyStateChanged = (a) => { };

        public Action<string> MoveError;

        //move queue events
        public Action<Queue<string>> MoveQueueReady;
        #endregion

        public Player(string name, Game game) : base(null)
        {
            _name = name;
            _id = "player:" + _name;
            _owner = this;
            _gameInstance = game;
            _health = _maxHealth = 30;
            _charges = _startTurnCount = 0;
            _isSimulated = false;
            _playerState = PlayerState.Idle;

            _cardsOnDeck = new List<Card>();
            _cardsOnHand = new List<Card>();
            _unitsOnField = new Dictionary<string, Unit>();
            _unitsDied = new Dictionary<string, Unit>();
            _cardsOnGraveyard = new List<Card>();

            _effectorStack = new EffectorStack();

            _unitSelector = new UnitSelector();
            ClearActions();
        }

        public string Name => _name;
        public int Charges => _charges;
        public bool IsSimulated { get => _isSimulated; set => _isSimulated = value; }
        public bool IsMyTurn { get => _gameInstance.CurrentPlayerTurn == this; }
        public List<Card> CardsOnDeck => _cardsOnDeck;
        public List<Card> CardsOnHand => _cardsOnHand;
        public List<Card> CardsOnGraveyard => _cardsOnGraveyard;
        public Dictionary<string, Unit> UnitsOnField => _unitsOnField;
        public Dictionary<string, Unit> UnitsDied => _unitsDied;
        public EffectorStack EffectorStack => _effectorStack;
        public Game Game => _gameInstance;
        public Player Opponent {
            get {
                return _gameInstance.GetOpponent(this);
            }
        }
        public UnitSelector UnitSelector => _unitSelector;
        public PlayerState PlayerState => _playerState;

        public void ClearActions()
        {
            RequestUseCard = (a) => { };
            RequestCommandAttack = (a, b) => { };

            CardAddedToHand = (a, b) => { };

            UnitKill = (a) => { };
            UnitSpawn = (a) => { };
            UnitDamaged = (a, b, c) => { };
            UnitReadyStateChanged = (a) => { };

            MoveError = (a) => { };

            MoveQueueReady = (a) => { };
        }

        public virtual void AddCardToHand(Card card, bool isDrawn = true)
        {
            dynamic parameters = new ExpandoObject();
            parameters.card = card;
            parameters.isDrawn = isDrawn;

            Action wrappedMethod = () =>
            {
                _cardsOnHand.Add(card);

                if (!IsSimulated)
                {
                    CardAddedToHand(card, isDrawn);
                }
            };

            _gameInstance.InterceptorService.RunInterceptor(InterceptorEvents.AddCardToHand, wrappedMethod, parameters);
        }

        public virtual void AddCardOnDeck(Card card)
        {
            _cardsOnDeck.Add(card);
        }

        public virtual void DrawCards(int count)
        {
            DrawCardEffector drawCardEffector = new DrawCardEffector(count);
            drawCardEffector.Player = this;

            Action wrappedMethod = () =>
            {
                _effectorStack.ApplyEffector(drawCardEffector);
            };

            dynamic parameters = new ExpandoObject();
            parameters.player = this;
            parameters.count = count;

            _gameInstance.InterceptorService.RunInterceptor(InterceptorEvents.DrawCard, wrappedMethod, parameters);
        }

        public virtual void KillUnit(Unit unit)
        {
            if (_unitsOnField.ContainsKey(unit.InstanceID))
            {
                _unitsOnField.Remove(unit.InstanceID);
                _unitsDied.Add(unit.InstanceID, unit);

                if (!IsSimulated)
                {
                    UnitKill(unit);
                }
            }
        }

        public virtual void SpawnUnit(Unit unit)
        {
            if (!_unitsOnField.ContainsKey(unit.InstanceID))
            {
                _unitsOnField.Add(unit.InstanceID, unit);
                _unitsDied.Remove(unit.InstanceID);

                if (!IsSimulated)
                {
                    UnitSpawn(unit);
                }
            }
        }

        public virtual void Ready()
        {
            if (_playerState == PlayerState.Ready) return;

            _gameInstance.InterceptorService.RegisterInterceptor(InterceptorEvents.UnitKill, new Events.Interceptor()
            {
                Priority = -1,
                After = (state) =>
                {
                    Unit unit = (Unit)state.Arguments.unit;
                    if (unit is Player && unit == this)
                    {
                        EndTurn(true);
                        _gameInstance.EndGame();
                        //set game over
                        state.Deny();
                    }
                    return state;
                }
            });

            //spawn player unit
            SpawnEffector spawnEffector = new SpawnEffector(this);
            spawnEffector.Player = this;
            _effectorStack.ApplyEffector(spawnEffector);


            //draw a card
            DrawCards(3);
            _playerState = PlayerState.Ready;
        }

        public virtual void StartTurn()
        {
            _startTurnEffectorIndex = _effectorStack.CurrentEffectorIndex;
            Action wrappedMethod = () =>
            {
                SetReady();
                RunTurnTimer();

                foreach (KeyValuePair<string, Unit> pair in _unitsOnField)
                {
                    pair.Value.SetReady();
                }


                _startTurnCount++;
                _charges = _startTurnCount;

                //draw a card
                DrawCards(1);
            };

            dynamic parameters = new ExpandoObject();
            parameters.currentTurn = this;
            parameters.previousTurn = _gameInstance.GetOpponent(this);

            _gameInstance.InterceptorService.RunInterceptor(InterceptorEvents.StartTurn, wrappedMethod, parameters);
        }

        public virtual void EndTurn(bool gameOver = false)
        {
            if (_playerState == PlayerState.Idle) return;
            if (!IsMyTurn) return;

            Action wrappedMethod = () =>
            {
                if (tokenSource != null)
                {
                    tokenSource.Cancel();
                }

                if (!gameOver)
                {
                    _gameInstance.NextTurn();
                }
            };

            dynamic parameters = new ExpandoObject();
            parameters.currentTurn = _gameInstance.GetOpponent(this);
            parameters.previousTurn = this;

            _gameInstance.InterceptorService.RunInterceptor(InterceptorEvents.EndTurn, wrappedMethod, parameters);
        }

        public virtual void ResetTurn()
        {
            _effectorStack.MoveTo(_startTurnEffectorIndex);
            StartTurn();
        }

        private void RunTurnTimer()
        {
            if (tokenSource != null)
            {
                tokenSource.Cancel();
            }

            tokenSource = new CancellationTokenSource();
            Task.Run(async () =>
            {
                await Task.Delay(_gameInstance.Settings.MaxTimePerRound * 1000, tokenSource.Token);
                if (!tokenSource.IsCancellationRequested)
                {
                    EndTurn();
                }
            });
        }

        public virtual void UseCard(Card card)
        {
            int index = _cardsOnHand.IndexOf(card);
            if (index == -1) return;
            UseCard(index);
        }

        public virtual void UseCard(int cardIndex)
        {
            if (_unitSelector.State == UnitSelectorState.Selecting) return;
            if (!IsMyTurn) return;

            Card selectedCard = _cardsOnHand[cardIndex];
            UseCardEffector useCardEffector = new UseCardEffector(selectedCard);

            if (_charges - selectedCard.Cost < 0)
            {
                MoveError("Insufficient charges.");
                return;
            }

            if (selectedCard.Type == CardType.Unit && _unitsOnField.Count >= 7)
            {
                MoveError("Battlefield is full.");
                return;
            }


            useCardEffector.Card = selectedCard;
            useCardEffector.Player = this;

            dynamic parameters = new ExpandoObject();
            parameters.card = selectedCard;

            if (selectedCard is IAimable)
            {
                Debug.Log($"{selectedCard.Name} is aimable!");
                IAimable aimable = (IAimable)selectedCard;
                Action<List<Unit>> wrappedMethod = (units) =>
                {
                    Action cardUseAction = () =>
                    {
                        Debug.Log($"Charge : {_charges}, cost: {selectedCard.Cost} = " + (_charges - selectedCard.Cost));
                        _charges -= selectedCard.Cost;
                        //todo optimize this
                        _effectorStack.ApplyEffector(useCardEffector);
                        foreach (Unit unit in units)
                        {
                            ((IAimable)selectedCard).Apply(unit);
                        }
                    };
                    _gameInstance.InterceptorService.RunInterceptor(InterceptorEvents.CardUse, cardUseAction, parameters);
                };

                _unitSelector.StartSelect(aimable.Targets, wrappedMethod, aimable.ValidateTarget);
            }
            else
            {
                Action cardUseAction = () =>
                {
                    _charges -= selectedCard.Cost;
                    _effectorStack.ApplyEffector(useCardEffector);
                    //apply card
                    selectedCard.Apply();
                };

                _gameInstance.InterceptorService.RunInterceptor(InterceptorEvents.CardUse, cardUseAction, parameters);
            }
        }

        public virtual void CommandAttack(string instanceID)
        {
            if (_unitSelector.State == UnitSelectorState.Selecting) return;
            if (!_unitsOnField.ContainsKey(instanceID)) return;

            Unit unitSelected = _unitsOnField[instanceID];

            if (unitSelected.State == UnitState.GettingReady) return;

            Action<List<Unit>> wrappedMethod = (units) =>
            {
                //_playerActionEvents.CommandAttack(unitSelected, units[0]);
                unitSelected.Attack(units[0], false);
            };

            _unitSelector.StartSelect(1, wrappedMethod, (unit) =>
            {
                //todo add status effect check
                return true;
            });
        }
    }
}
