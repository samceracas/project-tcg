using Boo.Lang;
using CardGame;
using CardGame.Cards.Base;
using Coffee.UIExtensions;
using Library.Extensions.Unity;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardScript : MonoBehaviour
{
    public enum CardFace
    {
        Back,
        Front
    };

    public enum CardUseHitboxState
    {
        NotIntersecting,
        Intersecting
    }

    public enum CardDetailsState
    {
        Hidden,
        Shown
    }

    public enum CardDragState
    {
        Dragging,
        NotDragging
    }

    [Header("UI References")]

    [SerializeField]
    private TMP_Text _cardText;

    [SerializeField]
    private TMP_Text _cardDescription;
    [SerializeField]
    private GameObject _cardDescriptionContainer;

    [SerializeField]
    private GameObject _attackContainer;
    [SerializeField]
    private TMP_Text _attackText;

    [SerializeField]
    private GameObject _chargeContainer;
    [SerializeField]
    private TMP_Text _chargeText;

    [SerializeField]
    private GameObject _healthContainer;
    [SerializeField]
    private TMP_Text _healthText;

    [SerializeField]
    private GameObject _cardBase;
    [SerializeField]
    private GameObject _cardBack;
    [SerializeField]
    private GameObject _cardFrame;

    [SerializeField]
    private UIDissolve[] _toDissolve;

    [SerializeField]
    private GameObject[] _toDisableBeforeDissolve;

    private PlayerScript _playerScript;
    private Card _card;
    private int _flipCount;
    private CardFace _cardFace;
    private RectTransform _cardUseHitbox;

    private Vector2 _beforeDragPosition;
    private Vector3 _beforeDragRotation;
    private CardUseHitboxState _cardUseHitboxState;

    private int _beforeHoverSiblingIndex;
    private CardDetailsState _cardDetailsState;
    private float _cardHoverCountdown;
    private bool _isHovered = false;

    private CardDragState _cardDragState;

    private float _screenWidthRatio = -1f;
    private float _screenHeightRatio = -1f;
    private RectTransform _cardRectTransform;


    public Card Card => _card;
    public Vector3 DefaultScale { get; set; }
    public Vector2 CardDimensions {
        get {
            if (_screenWidthRatio == -1f)
            {
                _screenWidthRatio = Screen.width / 1920f;
                _screenHeightRatio = Screen.height / 1080f;
            }

            int width = Mathf.FloorToInt(_cardRectTransform.rect.width * _screenWidthRatio);
            int height = Mathf.FloorToInt(_cardRectTransform.rect.height * _screenHeightRatio);
            return new Vector2(width, height);
        }
    }

    private void Update()
    {
        _cardHoverCountdown -= Time.deltaTime;
        if (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(_cardRectTransform, Input.mousePosition) && _isHovered)
            {
                ShowCardDetails();
            }
        }
    }

    private void ListenEvents()
    {

        EventTrigger trigger = GetComponentInChildren<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Drag;
        entry.callback.AddListener((data) => { DragCard((PointerEventData)data); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.BeginDrag;
        entry.callback.AddListener((data) => { BeginCardDrag((PointerEventData)data); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.EndDrag;
        entry.callback.AddListener((data) => { CheckUseCard((PointerEventData)data); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) =>
        {
            _isHovered = true;
        });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener((data) =>
        {
            _isHovered = false;
            HideCardDetails();
        });
        trigger.triggers.Add(entry);

        _card.EventCardCostUpdated += OnCardCostUpdated;
    }

    private void OnCardCostUpdated()
    {
        UpdateCardCostTexts();
    }

    private void UpdateCardCostTexts()
    {
        _chargeText.text = _card.Cost.ToString();
        _chargeText.color = Color.white;
        if (_card.CostModifiedState == -1)
        {
            _chargeText.color = Color.green;
        } else if (_card.CostModifiedState == 1)
        {
            _chargeText.color = Color.red;
        }
    }

    private void ToggleBackFace()
    {
        if (_cardFace == CardFace.Back)
        {
            _cardFace = CardFace.Front;
            _cardBack.SetActive(false);
            ShowStats();
        }
        else
        {
            _cardFace = CardFace.Back;
            _cardBase.transform.SetAsLastSibling();
            _cardBack.SetActive(true);
            HideStats();
        }
    }

    public void SetDescriptionVisible(bool visible, float speed = 0.3f)
    {
        if (visible)
        {
            LeanTween.alpha(_cardDescriptionContainer.GetComponent<RectTransform>(), 0.5f, speed);
            LeanTween.value(_cardDescription.alpha, 1f, speed).setOnUpdate((float value) =>
            {
                _cardDescription.alpha = value;
            });
        }
        else
        {
            LeanTween.alpha(_cardDescriptionContainer.GetComponent<RectTransform>(), 0f, speed);
            LeanTween.value(_cardDescription.alpha, 0f, speed).setOnUpdate((float value) =>
            {
                _cardDescription.alpha = value;
            });
        }
    }

    public void FlipCard(bool animate = false)
    {
        if (!animate)
        {
            transform.eulerAngles = new Vector3(
                transform.eulerAngles.x,
                0f,
                transform.eulerAngles.z
            );
            ToggleBackFace();
        }
        else
        {
            _flipCount = 1;
            LeanTween
                .rotateY(gameObject, 90f, 0.2f)
                .setLoopPingPong(1)
                .setOnComplete(() =>
                {
                    _flipCount++;
                    if (_flipCount % 2 == 0)
                    {
                        ToggleBackFace();
                    }
                })
                .setOnCompleteOnRepeat(true)
                .setEaseInQuart();
            LeanTween.rotateZ(gameObject, 40f, 0.2f).setLoopPingPong(1).setEaseInQuart();
        }
    }

    public void Dissolve(Action onComplete = null, float speed = 1f)
    {
        foreach (GameObject toDisable in _toDisableBeforeDissolve)
        {
            toDisable.SetActive(false);
        }

        UIDissolve[] dissolves = _toDissolve;
        LeanTween
            .value(gameObject, 0f, 1f, speed)
            .setOnUpdate((float value) =>
            {
                foreach (UIDissolve dissolve in dissolves)
                {
                    dissolve.effectFactor = value;
                }
            })
            .setOnComplete(onComplete != null ? onComplete : () => { });
    }

    public void UndoDissolve()
    {
        foreach (GameObject toDisable in _toDisableBeforeDissolve)
        {
            toDisable.SetActive(true);
        }
        UIDissolve[] dissolves = _toDissolve;
        foreach (UIDissolve dissolve in dissolves)
        {
            dissolve.effectFactor = 0f;
        }
    }

    public void ClearEvents()
    {
        EventTrigger trigger = GetComponentInChildren<EventTrigger>();
        trigger.triggers.Clear();
    }

    public void HideStats()
    {
        _chargeContainer.SetActive(false);
        _attackContainer.SetActive(false);
        _healthContainer.SetActive(false);
    }

    public void ShowStats()
    {
        _chargeContainer.SetActive(true);
        _attackContainer.SetActive(true);
        _healthContainer.SetActive(true);
    }

    public void SetFaceForward()
    {
        if (_cardFace == CardFace.Back)
        {
            ToggleBackFace();
        }
    }

    public void SetFaceBackward()
    {
        if (_cardFace == CardFace.Front)
        {
            HideStats();
            ToggleBackFace();
        }
    }

    public void ReturnToPreviousPosition()
    {
        _playerScript.ReArrangeCardsOnHand(0.2f);
        LeanTween.scale(gameObject, DefaultScale, 0.2f);
    }

    public void BeginCardDrag(PointerEventData pointerEventData)
    {
        if (!_card.IsUsable || GameScript.AnimationState == GameScript.GameAnimationState.Animating)
        {
            _cardDragState = CardDragState.NotDragging;
            pointerEventData.pointerDrag = null;
            return;
        }

        _cardUseHitboxState = CardUseHitboxState.NotIntersecting;
        _beforeDragPosition = transform.position;
        _beforeDragRotation = transform.eulerAngles;
        _cardDragState = CardDragState.Dragging;

        Cursor.visible = false;
        LeanTween.scale(gameObject, DefaultScale * 2f, 0.2f).setEaseOutCubic();
        LeanTween.rotate(gameObject, Vector3.zero, 0.5f).setEaseInOutBack();
    }

    public void CheckUseCard(PointerEventData baseEventData)
    {
        if (GameScript.AnimationState == GameScript.GameAnimationState.Animating || _cardDragState == CardDragState.NotDragging) return;

        Cursor.visible = true;
        _cardDragState = CardDragState.NotDragging;

        if (!_cardRectTransform.Overlaps(_cardUseHitbox, true) || !_card.IsUsable)
        {
            ReturnToPreviousPosition();
            return;
        }

        GameScript.AnimationState = GameScript.GameAnimationState.Animating;
        Dissolve(() =>
        {
            _playerScript.Player.RequestUseCard(_card);
        }, 0.2f);
    }

    public void DragCard(PointerEventData baseEventData)
    {
        if (GameScript.AnimationState == GameScript.GameAnimationState.Animating || _cardDragState == CardDragState.NotDragging || !_card.IsUsable) return;

        transform.position = baseEventData.position;
    }

    public void ShowCardDetails()
    {
        if (_cardDetailsState == CardDetailsState.Shown || _cardHoverCountdown > 0f) return;
        _cardDetailsState = CardDetailsState.Shown;

        _beforeHoverSiblingIndex = gameObject.transform.GetSiblingIndex();
        gameObject.transform.SetAsLastSibling();
        _cardHoverCountdown = 1f;

        LeanTween.rotate(gameObject, Vector3.zero, 0.2f).setEaseOutCubic();
        LeanTween.scale(gameObject, DefaultScale * 1.5f, 0.2f).setEaseOutCubic();
        LeanTween
            .moveY(gameObject, Screen.height * 0.17f, 0.2f)
            .setEaseOutCubic();
        SetDescriptionVisible(true);
    }

    public void HideCardDetails()
    {
        if (_cardDetailsState == CardDetailsState.Hidden) return;

        SetDescriptionVisible(false);
        gameObject.transform.SetSiblingIndex(_beforeHoverSiblingIndex);
        _cardDetailsState = CardDetailsState.Hidden;


        ReturnToPreviousPosition();
    }

    public void Ready(PlayerScript player, Card card)
    {
        _card = card;
        _playerScript = player;


        _attackContainer.SetActive(false);
        _healthContainer.SetActive(false);

        _cardText.text = _card.Name;
        _cardDescription.text = _card.Description;

        UpdateCardCostTexts();
        SetDescriptionVisible(false, 0f);
        
        if (_card.IsUnitCard)
        {
            _attackContainer.SetActive(true);
            _healthContainer.SetActive(true);

            _attackText.text = _card.Attack.ToString();
            _healthText.text = _card.Health.ToString();
        }

        _flipCount = 0;
        _cardFace = CardFace.Back;
        _cardDetailsState = CardDetailsState.Hidden;
        _cardDragState = CardDragState.NotDragging;


        _cardRectTransform = _cardFrame.GetComponent<RectTransform>();
        _cardUseHitbox = GameObject.FindGameObjectWithTag("CardUseHitbox").GetComponent<RectTransform>();

        ListenEvents();
        HideStats();
    }
}
