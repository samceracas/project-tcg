using CardGame;
using CardGame.Constants;
using CardGame.Effectors;
using CardGame.Events;
using CardGame.Players;
using CardGame.Units.Base;
using EZCameraShake;
using Force.DeepCloner;
using Library.Utils.Unity;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitScript : MonoBehaviour
{
    public class UnitTarget
    {
        public Action<UnitScript> onComplete = (a) => { };
        public Action<UnitScript> onInvalidTarget = (a) => { };
        public Action<UnitScript> onValidTargetMouseEnter = (a) => { };
        public Action<UnitScript> onValidTargetMouseExit = (a) => { };
        public Func<Unit, bool> validator;
        public UnitScript currentTarget;
    }

    public enum UnitAction
    {
        Idle,
        Attacking,
        Targeting
    };

    public enum HoveredState
    {
        NotHovered,
        Hovered
    }

    [Header("Unit Data")]

    [SerializeField, ReadOnly]
    private UnitAction _unitAction;

    [SerializeField, ReadOnly]
    private UnitScript _currentTarget;

    [SerializeField, ReadOnly]
    private HoveredState _hoveredState;

    [Header("References")]

    [SerializeField]
    private GameObject _playerIndicator;

    [SerializeField]
    private TMP_Text _damageText;

    [SerializeField]
    private TMP_Text _healthText;

    [SerializeField]
    private GameObject _unitSelectorGraphic;

    [SerializeField]
    private SpriteRenderer _unitSprite;

    [SerializeField]
    private GameObject _selectorArrow;


    [Header("Effect References")]

    [SerializeField]
    private GameObject _impactEffectPrefab;

    [SerializeField]
    private GameObject _sleepEffect;

    [SerializeField]
    private GameObject _statusNumberFloater;

    [SerializeField]
    private GameObject _deathMarker;

    [Header("Settings")]
    [SerializeField]
    private int _arrowInstanceCount = 10;




    private PlayerScript _playerScript;
    private Unit _unit;
    private SpawnPointScript _spawnPointScript;

    private GameObject[] _selectorArrows;

    private UnitTarget _targetingData;


    private void Start()
    {
        GameScript.AnimationState = GameScript.GameAnimationState.Idle;
    }


    public SpriteRenderer Sprite => _unitSprite;
    public Unit Unit => _unit;

    public void Ready(SpawnPointScript spawnPoint, PlayerScript player, Unit unit)
    {
        _targetingData = new UnitTarget();
        _hoveredState = HoveredState.NotHovered;
        _unitAction = UnitAction.Idle;
        _unit = unit;
        _playerScript = player;
        _spawnPointScript = spawnPoint;

        transform.localPosition = Vector3.zero;
        _unitSprite.flipX = !_playerScript.IsMe;
        _spawnPointScript.SetOccupied();

        _selectorArrows = new GameObject[_arrowInstanceCount];

        for (int i = 0; i < _arrowInstanceCount; i++)
        {
            _selectorArrows[i] = Instantiate(_selectorArrow, transform);
            _selectorArrows[i].GetComponentInChildren<SpriteRenderer>().enabled = false;
            if (i != _arrowInstanceCount - 1)
            {
                _selectorArrows[i].transform.localScale *= 0.7f;
            }
        }

        _sleepEffect.SetActive(true);
        UpdateTexts();
        ReadyEvents();

    }



    public void StartAttackTargetSelect()
    {
        if (GameScript.AnimationState == GameScript.GameAnimationState.Animating) return;
        if (!_playerScript.IsMe || !_playerScript.Player.IsMyTurn) return;
        if (_unit.State == UnitState.GettingReady) return;

        UnitTarget unitTarget = new UnitTarget();

        unitTarget.onComplete = (selectedUnit) =>
        {
            _unit.Owner.RequestCommandAttack(_unit, selectedUnit.Unit);
        };

        unitTarget.onInvalidTarget = (invalidTarget) =>
        {
            Debug.Log("Invalid target!");
        };

        unitTarget.validator = (unit) =>
        {
            return unit.Owner != _playerScript.Player && unit.State != UnitState.Dead;
        };

        unitTarget.onValidTargetMouseEnter = (unit) =>
        {
            unit.SetDeathMarkerActive(unit.Unit.Health - _unit.Damage <= 0);
            SetDeathMarkerActive(_unit.Health - unit.Unit.Damage <= 0);
        };

        unitTarget.onValidTargetMouseExit = (unit) =>
        {
            unit.SetDeathMarkerActive(false);
            SetDeathMarkerActive(false);
        };

        unitTarget.currentTarget = null;

        StartTargeting(unitTarget);
    }

    public void EndAttackTargetSelect()
    {
        EndTargetting();
    }

    public void SetDeathMarkerActive(bool active)
    {
        _deathMarker?.SetActive(active);
    }

    public void ShowTargetIndicator(bool hover = false, Action<UnitScript> onHover = null)
    {
        if (_hoveredState == HoveredState.Hovered) return;

        if ((!_playerScript.IsMe || !_playerScript.Player.IsMyTurn) && !hover) return;
        if (_unit.State == UnitState.GettingReady && !hover) return;

        LeanTween.cancel(_unitSelectorGraphic);
        _unitSelectorGraphic.transform.localScale = new Vector3(1f, 1f, 1f);
        LeanTween.alpha(_unitSelectorGraphic, 1f, 0.2f);
        LeanTween.scale(_unitSelectorGraphic, _unitSelectorGraphic.transform.localScale * 1.1f, 0.2f)
            .setEaseInOutCirc()
            .setLoopPingPong(-1);
        _hoveredState = HoveredState.Hovered;

        onHover?.Invoke(this);
    }

    public void HideTargetIndicator(Action<UnitScript> onExit = null)
    {
        if (_hoveredState == HoveredState.NotHovered) return;

        _hoveredState = HoveredState.NotHovered;

        LeanTween.cancel(_unitSelectorGraphic);
        LeanTween.alpha(_unitSelectorGraphic, 0f, 0f);
        _unitSelectorGraphic.transform.localScale = new Vector3(1f, 1f, 1f);
        onExit?.Invoke(this);


        if (_targetingData.currentTarget != null)
        {
            _targetingData.currentTarget.SetDeathMarkerActive(false);
        }
        SetDeathMarkerActive(false);
    }

    public void StartTargeting(UnitTarget targetData)
    {
        if (_unitAction != UnitAction.Idle) return;

        _targetingData = targetData;

        _unitAction = UnitAction.Targeting;

        for (int i = 0; i < _arrowInstanceCount; i++)
        {
            _selectorArrows[i].GetComponentInChildren<SpriteRenderer>().enabled = true;
        }
        Cursor.visible = false;
    }

    public void EndTargetting(bool cancel = false)
    {
        Cursor.visible = true;

        _unitAction = UnitAction.Idle;

        for (int i = 0; i < _arrowInstanceCount; i++)
        {
            _selectorArrows[i].GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        if (!cancel && _targetingData.currentTarget != null && _targetingData.validator(_targetingData.currentTarget.Unit))
        {
            _targetingData.onComplete(_targetingData.currentTarget);
        }

        if (_targetingData.currentTarget != null)
        {
            _targetingData.currentTarget.HideTargetIndicator(_targetingData.onValidTargetMouseExit);
            SetDeathMarkerActive(false);
        }

        _targetingData.onComplete = (a) => { };
        _targetingData.onInvalidTarget = (a) => { };
        _targetingData.currentTarget = null;
    }

    public virtual void DoDamage(LTSeq seq, Vector2 targetPos, UnitScript currentTarget, bool retaliate = false)
    {
        float targetXOffset = currentTarget.Sprite.bounds.extents.x * (!_playerScript.IsMe ? 1 : -1);
        if (retaliate)
        {
            targetXOffset = (currentTarget.Sprite.bounds.extents.x / 4) * (!_playerScript.IsMe ? -1 : 1);
        }
        seq.append(LeanTween.moveX(gameObject, targetPos.x - targetXOffset, 0.15f)
            .setLoopPingPong(1)
            .setEaseInQuint());
        seq.append(() =>
            Instantiate(_impactEffectPrefab, currentTarget.Sprite.transform));
        seq.append(() =>
        {
            _unit.Attack(currentTarget.Unit, retaliate);
        });
        seq.append(0.5f);
    }

    public virtual void AnimateAttack()
    {
        Vector3 targetPos = _currentTarget.transform.position;
        targetPos.x += (_currentTarget.Sprite.bounds.size.x) * (!_playerScript.IsMe ? 1 : -1);
        LTSeq seq = LeanTween.sequence();

        GameScript.AnimationState = GameScript.GameAnimationState.Animating;

        seq.append(LeanTween.move(gameObject, targetPos, 0.5f));
        seq.append(0.2f);

        DoDamage(seq, targetPos, _currentTarget);
        _currentTarget.DoDamage(seq, targetPos, this, true);

        seq.append(() => _unitSprite.flipX = !_unitSprite.flipX);
        seq.append(LeanTween.moveLocal(gameObject, Vector3.zero, 0.5f));
        seq.append(() => _unitSprite.flipX = !_unitSprite.flipX);
        seq.append(() => _unit.SetGettingReady());
        seq.append(() =>
        {
            _unitAction = UnitAction.Idle;
            GameScript.AnimationState = GameScript.GameAnimationState.Idle;
        });
    }

    public virtual void AnimateDeath()
    {
        float fadeTime = 1.5f;

        LeanTween.delayedCall(fadeTime / 2f, () =>
        {
            ShowStatusFloater(EffectType.Death, -1, true);
        });
        LeanTween.alphaCanvas(GetComponentInChildren<CanvasGroup>(), 0f, fadeTime).setDelay(fadeTime / 2f);
        LeanTween.alpha(_unitSprite.gameObject, 0f, fadeTime).setOnComplete(() =>
        {
            Kill();
        }).setDelay(fadeTime / 2f);
    }

    public void OnRequestCommandAttack(Unit dealer, Unit target)
    {
        if (_unitAction == UnitAction.Attacking || dealer != _unit) return;
        _currentTarget = _playerScript.Opponent.GetUnitOnField(target);
        _unitAction = UnitAction.Attacking;

        AnimateAttack();
    }

    public void OnUnitKill(Unit unit)
    {
        if (unit == _unit)
        {

            EventTrigger eventTrigger = GetComponentInChildren<EventTrigger>();
            eventTrigger.triggers.Clear();

            _sleepEffect.SetActive(false);
            _playerScript.RemoveUnit(this);
            Invoke("AnimateDeath", 1f);
        }
    }

    private void ShowStatusFloater(EffectType effectType, int damage = -1, bool iconOnly = false)
    {
        float scale = 1f;
        GameObject floaterInstance = Instantiate(_statusNumberFloater);
        floaterInstance.transform.position = transform.position;

        Vector3 pos = floaterInstance.transform.position;
        pos.y += _unitSprite.bounds.size.y;
        floaterInstance.transform.position = pos;

        if (damage >= 8f || effectType == EffectType.Death)
        {
            scale = 1.5f;
        }

        StatusNumberFloaterScript statusNumberFloater = floaterInstance.GetComponent<StatusNumberFloaterScript>();
        statusNumberFloater.Show(effectType, damage, iconOnly, scale, 1f, 1.2f);
    }

    private void OnUnitReadyStateChanged()
    {
        _sleepEffect.SetActive(_unit.State == UnitState.GettingReady);
    }

    private void OnUnitDamaged(int damage, EffectType damageSource)
    {
        TaskScheduler.Instance.Queue(() =>
        {
            LeanTween.color(_unitSprite.gameObject, Color.red, 0.2f).setLoopPingPong(1);
            UpdateTexts();
            Camera.main.GetComponent<CameraShaker>().ShakeOnce(3f, 10f, 0.1f, 0.1f);
            ShowStatusFloater(damageSource, damage);
        });
    }

    private void OnPlayerEndTurn()
    {
        TaskScheduler.Instance.Queue(() =>
        {
            EndTargetting(true);
        });
    }

    private void ReadyEvents()
    {
        _playerScript.Player.RequestCommandAttack += OnRequestCommandAttack;
        _playerScript.Player.EventUnitKill += OnUnitKill;
        _playerScript.Player.EventEndPlayerTurn += OnPlayerEndTurn;
        _unit.EventUnitReadyStateChanged += OnUnitReadyStateChanged;
        _unit.EventUnitDamaged += OnUnitDamaged;

        EventTrigger eventTrigger = GetComponentInChildren<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((eventData) => { ShowTargetIndicator(); });

        eventTrigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener((eventData) => { HideTargetIndicator(); });

        eventTrigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.BeginDrag;
        entry.callback.AddListener((eventData) => { StartAttackTargetSelect(); });

        eventTrigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.EndDrag;
        entry.callback.AddListener((eventData) => { EndAttackTargetSelect(); });

        eventTrigger.triggers.Add(entry);
    }

    private void UpdateTargeter()
    {
        if (_unitAction != UnitAction.Targeting) return;

        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 worldPoint2d = new Vector2(worldPoint.x, worldPoint.y);
        Vector2 currentPos = transform.position;

        currentPos.y += _unitSprite.bounds.extents.y;

        Vector2 diff = worldPoint2d - currentPos;

        float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

        float instanceCount = _arrowInstanceCount;

        for (float i = 0f; i < _arrowInstanceCount; i++)
        {
            _selectorArrows[(int)i].transform.position = currentPos + (diff * ((i + 1f) / instanceCount));
            _selectorArrows[(int)i].transform.rotation = q;
        }

        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null)
        {
            UnitScript targetedUnit = hit.collider.GetComponentInParent<UnitScript>();
            if (targetedUnit != null)
            {

                if (_targetingData.currentTarget != null && _targetingData.currentTarget != targetedUnit)
                {
                    _targetingData.currentTarget.HideTargetIndicator(_targetingData.onValidTargetMouseExit);
                    SetDeathMarkerActive(false);
                }

                if (_targetingData.validator(targetedUnit.Unit))
                {
                    _targetingData.currentTarget = targetedUnit;
                    _targetingData.currentTarget.ShowTargetIndicator(true, _targetingData.onValidTargetMouseEnter);
                }
                else
                {
                    if (_targetingData.currentTarget != targetedUnit)
                    {
                        _targetingData.currentTarget = targetedUnit;
                        _targetingData.onInvalidTarget(targetedUnit);
                    }
                }
            }
            else
            {
                if (_targetingData.currentTarget != null)
                {
                    _targetingData.currentTarget.HideTargetIndicator(_targetingData.onValidTargetMouseExit);
                    SetDeathMarkerActive(false);
                }
                _targetingData.currentTarget = null;
            }
        }
        else
        {
            if (_targetingData.currentTarget != null)
            {
                _targetingData.currentTarget.HideTargetIndicator(_targetingData.onValidTargetMouseExit);
                SetDeathMarkerActive(false);
            }
            _targetingData.currentTarget = null;
        }
    }

    private void Kill()
    {
        _playerScript.Player.RequestCommandAttack -= OnRequestCommandAttack;
        _playerScript.Player.EventUnitKill -= OnUnitKill;
        _playerScript.Player.EventEndPlayerTurn -= OnPlayerEndTurn;
        _unit.EventUnitReadyStateChanged -= OnUnitReadyStateChanged;
        _unit.EventUnitDamaged -= OnUnitDamaged;

        _spawnPointScript.SetUnOccupied();
        Destroy(gameObject);
    }

    private void UpdateTexts()
    {
        _damageText.text = _unit.Damage.ToString();
        _healthText.text = _unit.Health + "/" + _unit.MaxHealth;
    }

    private void Update()
    {
        UpdateTargeter();
    }
}
