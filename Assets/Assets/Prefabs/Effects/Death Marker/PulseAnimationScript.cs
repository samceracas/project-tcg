using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseAnimationScript : MonoBehaviour
{
    [SerializeField]
    private float _scaleOffset = 0.1f;

    [SerializeField]
    private float _duration = 0.5f;

    [SerializeField]
    private int _pulseCount = -1;

    [SerializeField]
    private bool _destroyOnComplete = false;

    // Start is called before the first frame update
    void Start()
    {
        OnEnable();
    }

    private void OnDisable()
    {
        LeanTween.cancel(gameObject);
    }

    private void OnEnable()
    {
        LeanTween.scale(gameObject, gameObject.transform.localScale * (1f + _scaleOffset), _duration)
            .setEaseInExpo()
            .setLoopPingPong(_pulseCount)
            .setOnCompleteOnRepeat(false)
            .setDestroyOnComplete(_destroyOnComplete);
    }
}
