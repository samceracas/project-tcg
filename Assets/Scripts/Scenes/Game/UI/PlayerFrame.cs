using Boo.Lang;
using CardGame;
using CardGame.Cards.Base;
using CardGame.Constants;
using CardGame.Events;
using CardGame.Players;
using CardGame.Units.Base;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class PlayerFrame : MonoBehaviour
{
    [Header("Player Information")]
    [SerializeField]
    private bool _isMe = false;

    [Header("References")]

    [SerializeField]
    private GameScript _gameController;

    [SerializeField]
    private TMP_Text _chargeCountText;

    [SerializeField]
    private TMP_Text _deckCountText;

    [SerializeField]
    private TMP_Text _turnText;

    [SerializeField]
    private GameObject _frameContainer;

    private PlayerScript _playerScript;
    private List<GameObject> _charges;

    public bool IsMe => _isMe;

    public void Ready(PlayerScript playerScript)
    {
        _playerScript = playerScript;
        _charges = new List<GameObject>();

        _gameController.Game.EventGameStart += () =>
        {
            foreach (Player player in _gameController.Game.Players)
            {
                UpdateCardInDeckCount(player);
            }
        };

        _gameController.Game.InterceptorService.RegisterInterceptor(InterceptorEvents.CardUse, new Interceptor()
        {
            AllowRunInSimulation = false,
            Priority = -1,
            After = (state) =>
            {
                Card usedCard = state.Arguments.card;

                if (usedCard.Player == _playerScript.Player)
                {
                    TaskScheduler.Instance.Queue(new UnityTask(delegate 
                    {
                        UpdateCardInDeckCount(_playerScript.Player);
                        UpdateChargeCount(_playerScript.Player);
                    }));
                }
                return state;
            }
        });


        _gameController.Game.InterceptorService.RegisterInterceptor(InterceptorEvents.StartTurn, new Interceptor()
        {
            AllowRunInSimulation = false,

            After = (state) =>
            {
                Player currentTurn = state.Arguments.currentTurn;

                if (currentTurn == _playerScript.Player)
                {
                    TaskScheduler.Instance.Queue(() =>
                    {
                        UpdateCardInDeckCount(_playerScript.Player);
                        UpdateChargeCount(_playerScript.Player);
                    });
                }
                TaskScheduler.Instance.Queue(new UnityTask(delegate { _turnText.text = currentTurn == _playerScript.Player && _isMe ? "Your turn" : $"{currentTurn.Name}'s turn."; }));
                return state;
            }
        });
    }

    public void EndTurn()
    {
        if (_isMe)
        {
            _playerScript.Player.EndTurn();
        }
    }

    public void AddUnitFrame(Game game, Unit unit, GameObject prefab)
    {
        GameObject frame = Instantiate(prefab, _frameContainer.transform);
        frame.GetComponent<UnitFrame>().Ready(game, unit);
    }

    private void UpdateChargeCount(Player player)
    {
        _chargeCountText.text = player.Charges.ToString();
    }

    private void UpdateCardInDeckCount(Player player)
    {
        int total = player.CardsOnHand.Count + player.CardsOnGraveyard.Count + player.CardsOnDeck.Count;
        _deckCountText.text = player.CardsOnHand.Count + "/" + total;
    }
}
