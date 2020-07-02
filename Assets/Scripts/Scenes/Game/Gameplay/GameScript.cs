using CardGame;
using CardGame.Players;
using System;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameScript : MonoBehaviour
{
    public enum GameState
    {
        Waiting,
        InProgress,
        Finished
    }

    public enum GameAnimationState
    {
        Idle,
        Animating
    }

    [Header("Info")]
    [SerializeField]
    private GameObject _playerPrefab;

    [SerializeField]
    private GameState _state = GameState.Waiting;

    [SerializeField]
    private GameSettings _gameSettings;

    [Header("Events")]
    public UnityEvent EventGameWaiting;
    public UnityEvent EventGameInProgress;
    public UnityEvent EventGameFinished;

    [Header("UI References")]
    [SerializeField]
    private TMP_Text _countdownText;


    private static GameAnimationState _gameAnimationState;

    private Game _game;
    private CancellationTokenSource _countdownTokenSource;
    private int _roundCountdown;
    private CountdownTimer.Entry _roundTimer;

    public Game Game => _game;
    public static GameAnimationState AnimationState { get => _gameAnimationState; set => _gameAnimationState = value; }

    // Start is called before the first frame update
    private void Start()
    {
        LeanTween.init(1000, 1000);
        _gameAnimationState = GameAnimationState.Idle;
        _game = new Game(DateTime.Now.ToString(), _gameSettings);
        SetState(GameState.Waiting);

        _roundTimer = new CountdownTimer.Entry();
        _roundTimer.CountdownTime = _game.Settings.MaxTimePerRound;

        _roundTimer.OnUpdate += (entry) =>
        {
            _countdownText.text = _roundTimer.TimeInSeconds.ToString("D2");
        };

        _roundTimer.OnComplete += (entry) =>
        {
            if (_game.NextTurn())
            {
                _roundTimer.Reset();
            } else
            {
                _roundTimer.Stop();
            }
        };

        _game.EventGameEnd += () =>
        {
            _roundTimer.Stop();
            CountdownTimer.Instance.RemoveEntry(_roundTimer);
            Debug.Log("--------------- END -------------");
        };

        AddPlayer("sam", "dragon_lord_class", true);
        AddPlayer("sam 2", "dragon_lord_class_bot");

        ReadyPlayers();

        foreach (Player player in _game.Players)
        {
            player.EventEndPlayerTurn += () =>
            {
                _roundTimer.Reset();
            };
        }

        Invoke("StartGame", 1f);
    }

    private void ReadyPlayers()
    {
        _game.ReadyPlayers();
    }

    private void StartGame()
    {
        TaskScheduler.Instance.Queue(new UnityTask(delegate
        {
            CountdownTimer.Instance.AddEntry(_roundTimer);
            _game.StartGame();
            SetState(GameState.InProgress);
        }));
    }

    private void OnDestroy()
    {
        Debug.Log("Disposing current game");
        _game.Dispose();
    }

    public void AddPlayer(string name, string className, bool isMe = false)
    {
        GameObject playerPrefab = Instantiate(_playerPrefab);
        PlayerScript player = playerPrefab.GetComponent<PlayerScript>();
        player.Initialize(name, className, isMe, _game);
        _game.AddPlayer(player.Player);
    }

    public void SetState(GameState state)
    {
        _state = state;

        switch (state)
        {
            case GameState.Waiting:
                _roundCountdown = 0;
                EventGameWaiting?.Invoke();
                break;
            case GameState.InProgress:
                EventGameInProgress?.Invoke();
                break;
            case GameState.Finished:
                EventGameFinished?.Invoke();
                break;
        }
    }
}
