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

        playerScript.Player.StartPlayerTurn += () =>
        {
            TaskScheduler.Instance.Queue(() =>
            {
                if (playerScript.IsMe)
                {
                    TaskScheduler.Instance.Queue(() =>
                    {
                        UpdateCardInDeckCount(_playerScript.Player);
                        UpdateChargeCount(_playerScript.Player);
                    });
                }
                _turnText.text = _playerScript.Player.IsMyTurn && _isMe ? "Your turn" : $"{_playerScript.Player.Name}'s turn.";
            });

        };

        playerScript.Player.CardUsed = (card) =>
        {
            TaskScheduler.Instance.Queue(() => 
            {
                UpdateCardInDeckCount(_playerScript.Player);
                UpdateChargeCount(_playerScript.Player);
            });
        };

        _gameController.Game.EventGameStart += () =>
        {
            foreach (Player player in _gameController.Game.Players)
            {
                UpdateCardInDeckCount(player);
            }
        };
    }

    public void EndTurn()
    {
        if (_isMe && GameScript.AnimationState == GameScript.GameAnimationState.Idle)
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
        _deckCountText.text = player.CardsOnDeck.Count + "/" + player.TotalCards;
    }
}
