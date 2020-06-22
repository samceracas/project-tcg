﻿using Boo.Lang;
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

    private CardDetailsState _cardDetailsState;

    public Card Card => _card;
    public Vector3 DefaultScale { get; set; }
    public Vector2 CardDimensions {
        get {
            var rectTransform = _cardFrame.GetComponent<RectTransform>();
            float widthFactor = Screen.width / 1920f;
            float heithFactor = Screen.height / 1080f;
            int width = Mathf.FloorToInt(rectTransform.rect.width * widthFactor);
            int height = Mathf.FloorToInt(rectTransform.rect.height * heithFactor);
            return new Vector2(width, height);
        }
    }

    private void Start()
    {
        _flipCount = 0;
        _cardFace = CardFace.Back;
        _cardDetailsState = CardDetailsState.Hidden;

        HideStats();

        _cardUseHitbox = GameObject.FindGameObjectWithTag("CardUseHitbox").GetComponent<RectTransform>();

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
        entry.callback.AddListener((data) => { ShowCardDetails(); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener((data) => { HideCardDetails(); });
        trigger.triggers.Add(entry);
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
            LeanTween.alpha(_cardDescriptionContainer.GetComponent<RectTransform>(), 1f, speed);
            LeanTween.alphaText(_cardText.rectTransform, 1f, speed);
        }
        else
        {
            LeanTween.alpha(_cardDescriptionContainer.GetComponent<RectTransform>(), 0f, speed);
            LeanTween.alphaText(_cardText.rectTransform, 0f, speed);
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
            GameScript.AnimationState = GameScript.GameAnimationState.Animating;
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
                    if (_flipCount % 3 == 0)
                    {
                        GameScript.AnimationState = GameScript.GameAnimationState.Idle;
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
        GameScript.AnimationState = GameScript.GameAnimationState.Animating;
        _playerScript.ReArrangeCardsOnHand(0.2f);
        LeanTween.scale(gameObject, DefaultScale, 0.2f);
        LeanTween.delayedCall(0.2f, () =>
        {
            GameScript.AnimationState = GameScript.GameAnimationState.Idle;
        });
    }

    public void BeginCardDrag(PointerEventData pointerEventData)
    {
        if (!_card.IsUsable()) return;

        _cardUseHitboxState = CardUseHitboxState.NotIntersecting;
        _beforeDragPosition = transform.position;
        _beforeDragRotation = transform.eulerAngles;

        Cursor.visible = false;
        LeanTween.scale(gameObject, DefaultScale * 2f, 0.2f).setEaseOutCubic();
        LeanTween.rotate(gameObject, Vector3.zero, 0.5f).setEaseInOutBack();
    }

    public void CheckUseCard(PointerEventData baseEventData)
    {
        Cursor.visible = true;
        RectTransform rectTransform = GetComponent<RectTransform>();

        if (!rectTransform.Overlaps(_cardUseHitbox, true) || !_card.IsUsable())
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

    public void ShowCardDetails()
    {
        if (_cardDetailsState == CardDetailsState.Shown) return;
        _cardDetailsState = CardDetailsState.Shown;
        
        gameObject.transform.SetAsLastSibling();

        LeanTween.rotate(gameObject, Vector3.zero, 0.2f).setEaseInOutBack();
        LeanTween.scale(gameObject, DefaultScale * 1.5f, 0.2f).setEaseInOutBack();
        LeanTween.moveY(gameObject, transform.position.y + ((CardDimensions.y * DefaultScale.y * 1.5f) / 2.4f), 0.2f).setEaseInOutBack();
    }

    public void HideCardDetails()
    {
        if (_cardDetailsState == CardDetailsState.Hidden) return;

        _cardDetailsState = CardDetailsState.Hidden;
        ReturnToPreviousPosition();
    }

    public void DragCard(PointerEventData baseEventData)
    {
        if (GameScript.AnimationState == GameScript.GameAnimationState.Animating || !_card.IsUsable()) return;

        transform.position = baseEventData.position;
    }

    public void Ready(PlayerScript player, Card card)
    {
        _card = card;
        _playerScript = player;


        _attackContainer.SetActive(false);
        _healthContainer.SetActive(false);

        _cardText.text = _card.Name;
        _cardDescription.text = _card.Description;
        _chargeText.text = _card.Cost.ToString();


        SetDescriptionVisible(false, 0f);
        if (_card.Type == CardType.Unit)
        {
            _attackContainer.SetActive(true);
            _healthContainer.SetActive(true);

            _attackText.text = _card.Attack.ToString();
            _healthText.text = _card.Health.ToString();
        }
    }
}
