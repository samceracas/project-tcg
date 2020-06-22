using CardGame;
using CardGame.Cards.Base;
using CardGame.Constants;
using CardGame.Events;
using CardGame.Players;
using CardGame.Units.Base;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Player Information")]

    [SerializeField]
    private bool _isMe = false;

    [SerializeField]
    private string _playerName;

    [SerializeField]
    private List<SpawnPointScript> _spawnPoints;

    [SerializeField, ReadOnly]
    private List<UnitScript> _unitsOnField;

    [SerializeField, ReadOnly]
    private List<GameObject> _cardsOnHand;

    [SerializeField, ReadOnly]
    private PlayerScript _opponent;

    [Header("Class Information")]

    [SerializeField]
    private string _className;

    [SerializeField]
    private int _classTier;

    [Header("Databases")]

    [SerializeField]
    private UnitMap _unitMap;

    [SerializeField]
    private ClassMap _cardMap;

    private Game _game;
    private Player _player;
    private GameObject _cardContainer, _deckObject, _gameCanvas;
    private PlayerFrame _playerFrame;

    private float _addToHandDelay = 0f;

    public bool IsMe => _isMe;
    public string PlayerName => _player.Name;
    public Player Player => _player;
    public Game Game => _game;
    public PlayerScript Opponent {
        get {
            if (_opponent == null)
            {
                string enemyTag = !_isMe ? "CurrentPlayer" : "EnemyPlayer";
                _opponent = GameObject.FindGameObjectWithTag(enemyTag).GetComponent<PlayerScript>();
            }
            return _opponent;
        }
    }

    public void Initialize(string name, string classCode, bool isMe, Game game)
    {
        if (_player == null)
        {
            _player = InstantiatePlayerClass(name, game, classCode);
            if (_player == null)
            {
                Debug.LogError("Could not instantiate player, probable cause is a mismatched class name.");
                return;
            }
            _isMe = isMe;
            _playerName = PlayerName;
            _game = game;
            _unitsOnField = new List<UnitScript>();

            tag = _isMe ? "CurrentPlayer" : "EnemyPlayer";
            _className = ((IPlayerClass)_player).ClassName;
            _classTier = ((IPlayerClass)_player).ClassTier;

            _cardContainer = GameObject.FindGameObjectWithTag("CardContainer");
            _gameCanvas = GameObject.FindGameObjectWithTag("GameCanvas");
            _deckObject = GameObject.FindGameObjectWithTag("Deck");

            PlayerFrame[] playerFrames = GameObject.FindObjectsOfType<PlayerFrame>();
            foreach (PlayerFrame playerFrame in playerFrames)
            {
                if (playerFrame.IsMe == _isMe)
                {
                    _playerFrame = playerFrame;
                }
            }

            _cardsOnHand = new List<GameObject>();

            ReadySpawnPoints();
            ReadyEvents();
            ReadyPlayerFrame();
        }
    }

    public void RemoveUnit(UnitScript unit)
    {
        _unitsOnField.Remove(unit);
    }

    public UnitScript GetUnitOnField(Unit unit)
    {
        return _unitsOnField.Find(unitScript => unitScript.Unit == unit);
    }


    private void SpawnUnit(UnitData entry, int spawnIndex, Unit unit)
    {
        SpawnPointScript spawn = null;
        for (int i = 0; i < _spawnPoints.Count; i++)
        {
            SpawnPointScript spawnPoint = _spawnPoints[i];
            if (!spawnPoint.IsOccupied)
            {
                spawn = spawnPoint;
                break;
            }
        }

        if (spawn == null) return;

        GameObject obj = Instantiate(entry.UnitPrefab, spawn.transform);
        UnitScript unitScript = obj.GetComponent<UnitScript>();

        _unitsOnField.Add(unitScript);
        _playerFrame.AddUnitFrame(_game, unit, entry.UnitFrame);

        unitScript.Ready(spawn, this, unit);
    }


    public void ReArrangeCardsOnHand(float speed)
    {
        if (_cardsOnHand.Count <= 0) return;
        float totalCardCount = _cardsOnHand.Count;
        float maxCardCount = 10f;
        float cardWidth = _cardsOnHand[0].GetComponent<CardScript>().CardDimensions.x * 0.65f;
        float startX = (_gameCanvas.transform.position.x) - ((totalCardCount * (cardWidth / 2)) / 2);
        float quarterWidth = (cardWidth / 4f);
        float cardsCountPercent = (totalCardCount) / maxCardCount;
        float totalTwist = 30f;
        if (totalCardCount - 1 <= 0) totalTwist = 0f;
        float twistPerCard = totalTwist / totalCardCount;
        float startTwist = -1f * (totalTwist / 2f);

        startX += (quarterWidth * cardsCountPercent) * totalCardCount / 2;

        int i = 0;
        foreach (GameObject cardInstance in _cardsOnHand)
        {
            float twistForThisCard = startTwist + (i * twistPerCard);
            float newX = startX + ((i) * (cardWidth / 2f));

            GameScript.AnimationState = GameScript.GameAnimationState.Animating;

            LeanTween
                .move(cardInstance, new Vector3(
                    newX,
                    _cardContainer.transform.position.y - 25f,
                    _cardContainer.transform.position.z
                ), speed)
                .setEaseInOutCubic();
            LeanTween
                .rotateZ(cardInstance, -twistForThisCard, speed)
                .setEaseInOutCubic()
                .setOnComplete(() =>
                {
                    GameScript.AnimationState = GameScript.GameAnimationState.Idle;
                });
            startX -= quarterWidth * cardsCountPercent;
            i++;
        }
    }

    private void AnimateAndUseCard(Card card)
    {
        GameObject toRemove = null;
        foreach (GameObject cardObject in _cardsOnHand)
        {
            CardScript cardScript = cardObject.GetComponent<CardScript>();
            if (cardScript.Card == card)
            {
                toRemove = cardObject;
                LeanTween.cancel(cardObject);

                cardScript.SetFaceForward();
                cardScript.UndoDissolve();

                cardObject.transform.position = new Vector2(Screen.width / 2, Screen.height / 2);
                cardObject.transform.eulerAngles = Vector3.zero;
                cardObject.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);

                GameScript.AnimationState = GameScript.GameAnimationState.Animating;
                LTSeq seq = LeanTween.sequence();
                seq.append(LeanTween
                    .scale(cardObject, new Vector3(0.55f, 0.55f, 0.55f), 0.3f)
                    .setEaseInCirc());
                seq.append(0.8f);
                seq.append(() => cardScript.Dissolve(() =>
                {
                    _player.UseCard(card);
                    GameScript.AnimationState = GameScript.GameAnimationState.Idle;
                    Destroy(cardObject);
                }, 0.3f));
                break;
            }
        }

        if (toRemove != null)
        {
            _cardsOnHand.Remove(toRemove);
        }

        ReArrangeCardsOnHand(0.5f);
    }

    private void ReadySpawnPoints()
    {
        string prefix = _isMe ? "Blue_" : "Red_";

        _spawnPoints = new List<SpawnPointScript>();

        _spawnPoints.Add(GameObject.FindGameObjectWithTag(prefix + "BossSpawn")
            .GetComponent<SpawnPointScript>());

        for (int i = 1; i <= 6; i++)
        {
            SpawnPointScript spawnPointScript = GameObject
                .FindGameObjectWithTag(prefix + "Spawn " + i)
                .GetComponent<SpawnPointScript>();

            _spawnPoints.Add(spawnPointScript);
        }
    }

    private void ReadyPlayerFrame()
    {
        PlayerFrame playerFrame = null;
        PlayerFrame[] playerFrames = GameObject.FindObjectsOfType<PlayerFrame>();

        foreach (PlayerFrame frame in playerFrames)
        {
            if (frame.IsMe == _isMe)
            {
                playerFrame = frame;
                break;
            }
        }

        if (playerFrame != null)
        {
            playerFrame.Ready(this);
        }
    }

    private Player InstantiatePlayerClass(string playerName, Game game, string classID)
    {
        UnitData entry = _unitMap.GetUnitDataByID(classID);
        if (entry == null) return null;

        return (Player)Activator.CreateInstance(
            Type.GetType("Gameplay.Units.Classes." + entry.ClassName), new object[] {
                playerName,
                game
        });
    }

    private Card InstantiateCardClass(string className)
    {
        return (Card)Activator.CreateInstance(
            Type.GetType("Gameplay.Cards." + className), new object[] {
                _player
            });
    }


    private void OnRequestUseCard(Card drawnCard)
    {

        if (drawnCard.Player == _player && _isMe)
        {
            AnimateAndUseCard(drawnCard);
        }

        if (!_isMe && drawnCard.Player == _player)
        {
            ClassEntry cardEntry = _cardMap.GetEntryByKey(drawnCard.ID);
            TaskScheduler.Instance.Queue(new UnityTask(delegate
            {

                if (cardEntry != null)
                {
                    GameObject cardInstance = Instantiate(cardEntry.data.prefab, _gameCanvas.transform);
                    CardScript cardScript = cardInstance.GetComponent<CardScript>();
                    //@todo add config for default card scale
                    cardScript.DefaultScale = new Vector3(0.45f, 0.45f, 0.45f);
                    cardScript.Ready(this, drawnCard);
                    cardScript.ShowStats();

                    _cardsOnHand.Add(cardInstance);

                    AnimateAndUseCard(drawnCard);
                }
            }));
        }
    }

    private void OnCardAddedToHand(Card card, bool isDrawn)
    {
        if (!_isMe) return;
        if (card.Player != _player) return;

        ClassEntry cardEntry = _cardMap.GetEntryByKey(card.ID);

        if (cardEntry != null)
        {
            GameObject cardInstance = Instantiate(cardEntry.data.prefab, _gameCanvas.transform);
            CardScript cardScript = cardInstance.GetComponent<CardScript>();
            cardScript.DefaultScale = new Vector3(0.45f, 0.45f, 0.45f);
            cardScript.Ready(this, card);

            cardInstance.transform.position = _deckObject.transform.position;

            cardInstance.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            float delay = 0.5f;
            
            LeanTween
                .scale(cardInstance, cardScript.DefaultScale, delay)
                .setDelay(_addToHandDelay)
                .setOnComplete(() =>
                {
                    _addToHandDelay -= delay;
                })
                .setOnStart(() =>
                {
                    _cardsOnHand.Add(cardInstance);
                    ReArrangeCardsOnHand(delay);
                    cardScript.FlipCard(true);
                });
            _addToHandDelay += delay;
        }
    }


    private void OnMoveQueueReady(Queue<string> commands)
    {
        TaskScheduler.Instance.Queue(() =>
        {
            LTSeq sequence = LeanTween.sequence();
            while (commands.Any() && _player.Game.GameState == GameState.InProgress && _player.IsMyTurn)
            {
                string unparsedCommand = commands.Dequeue();
                string[] tokens = unparsedCommand.Split(':');
                switch (tokens[0])
                {
                    case "attack":
                        Player opponent = _player.Opponent;
                        Unit target = opponent;
                        Unit dealer = _player.UnitsOnField[tokens[1]];
                        if (!tokens[2].Equals("opponent"))
                        {
                            target = opponent.UnitsOnField[tokens[2]];
                        }
                        sequence.append(() => _player.RequestCommandAttack(_player.UnitsOnField[tokens[1]], target));
                        sequence.append(2f);
                        break;
                    case "use_card":
                        sequence.append(() => _player.RequestUseCard(_player.CardsOnHand[int.Parse(tokens[1])]));
                        sequence.append(2f);
                        break;
                }
                sequence.append(0.5f);
            }

            if (_player.Game.GameState == GameState.InProgress)
            {
                sequence.append(() => _player.EndTurn());
            }
        });
    }

    private void OnUnitSpawn(Unit unit)
    {
        if (unit.Owner == _player)
        {
            bool isPlayer = unit is Player;
            string entryKey = isPlayer ? ((IPlayerClass)_player).ClassID : unit.ID;

            int spawnIndex = unit.Owner.UnitsOnField.Count - 1;
            UnitData entry = _unitMap.GetUnitDataByID(entryKey);
            SpawnUnit(entry, spawnIndex, unit);
        }
    }

    private void ReadyEvents()
    {
        _player.CardAddedToHand += OnCardAddedToHand;
        _player.RequestUseCard += OnRequestUseCard;
        _player.MoveQueueReady += OnMoveQueueReady;
        _player.UnitSpawn += OnUnitSpawn;
    }
}
