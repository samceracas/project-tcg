using CardGame.Effectors;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusNumberFloaterScript : MonoBehaviour
{

    [Header("References")]

    [SerializeField]
    private Image _statusIcon;

    [SerializeField]
    private TMP_Text _numberText;

    [SerializeField]
    private List<Sprite> _icons;

    [SerializeField, ReadOnly]
    private EffectType _statusType;

    public void Show(EffectType type, int num = -1, bool iconOnly = false, float scaleMultiplier = 1f, float offsetHeight = 1.5f, float duration = 2f, Color color = new Color())
    {
        CanvasGroup canvasGroup = GetComponentInChildren<CanvasGroup>();
        _statusType = type;

        _statusIcon.sprite = _icons[(int)type];
        _numberText.text = num.ToString();
        _numberText.color = color == null ? Color.white : color;
        transform.localScale = new Vector3(1f, 1f, 1f) * scaleMultiplier;

        if (iconOnly)
        {
            _statusIcon.rectTransform.offsetMax = Vector2.zero;
            _statusIcon.rectTransform.offsetMin = Vector2.zero;
            _statusIcon.rectTransform.anchorMin = Vector2.zero;
            _statusIcon.rectTransform.anchorMax = new Vector2(1f, 1f);
            _statusIcon.rectTransform.pivot = new Vector2(0.5f, 0.5f);

            _numberText.gameObject.SetActive(false);
        }
        float delay = duration / 2f;
        LeanTween.moveY(gameObject, transform.position.y + offsetHeight, duration);
        LeanTween.alphaCanvas(canvasGroup, 0f, 0.1f).setDelay(duration - (duration / 8f)).setOnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}
