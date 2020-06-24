using CardGame;
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

    [Header("Events")]
    public UnityEvent EventGameWaiting;
    public UnityEvent EventGameInProgress;
    public UnityEvent EventGameFinished;

    [Header("UI References")]
    [SerializeField]
    private TMP_Text _countdownText;

    private Game _game;
    private CancellationTokenSource _countdownTokenSource;
    private int _roundCountdown;
    private static GameAnimationState _gameAnimationState;

    public Game Game => _game;
    public static GameAnimationState AnimationState { get => _gameAnimationState; set => _gameAnimationState = value; }

    // Start is called before the first frame update
    private void Start()
    {
        LeanTween.init(1000, 1000);
        _gameAnimationState = GameAnimationState.Idle;
        _game = new Game(DateTime.Now.ToString());
        SetState(GameState.Waiting);

        _game.EventStartPlayerTurn += (a) => ResetRoundTimer();
        _game.EventGameEnd += () =>
        {
            StopRoundTimer();
            Debug.Log("--------------- END -------------");
        };

        AddPlayer("sam", "dragon_lord_class", true);
        AddPlayer("sam 2", "dragon_lord_class_bot");

        ReadyPlayers();
        Invoke("StartGame", 3f);
    }

    private void Update()
    {
        _countdownText.text = _roundCountdown.ToString("D2");
    }

    private void ReadyPlayers()
    {
        _game.ReadyPlayers();
    }

    private void StartGame()
    {
        TaskScheduler.Instance.Queue(new UnityTask(delegate {
            _game.StartGame();
            SetState(GameState.InProgress);
        }));
    }

    private void StartRoundTimer()
    {
        _countdownTokenSource = new CancellationTokenSource();
        _roundCountdown = _game.Settings.MaxTimePerRound;
        Task.Run(async () =>
        {
            while (_game.GameState == CardGame.GameState.InProgress)
            {
                await Task.Delay(1000, _countdownTokenSource.Token);
                _roundCountdown = Mathf.Clamp(--_roundCountdown, 0, _game.Settings.MaxTimePerRound);
            }
        });
    }

    private void StopRoundTimer()
    {
        if (_countdownTokenSource == null) return;
        _countdownTokenSource.Cancel();
    }

    private void ResetRoundTimer()
    {
        StopRoundTimer();
        StartRoundTimer();
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
