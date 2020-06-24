using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LTQueue
{

    protected static LTQueue _instance;

    public static LTQueue Instance {
        get {
            if (_instance == null)
            {
                _instance = new LTQueue();
            }
            return _instance;
        }
    }


    public enum SequenceLockState
    {
        Locked,
        Unlocked
    }

    protected SequenceLockState _lockState;
    protected LTSeq _globalSequence;

    protected LTQueue()
    {
        _lockState = SequenceLockState.Unlocked;
        _globalSequence = LeanTween.sequence();
    }

    public SequenceLockState LockState => _lockState;
    public LTSeq GlobalSequence => _globalSequence;

    public void LockedSequence(LTSeq seq)
    {
        if (_lockState == SequenceLockState.Locked) return;

        _lockState = SequenceLockState.Locked;

        seq.append(() =>
        {
            _lockState = SequenceLockState.Unlocked;
        });
    }

    public void Unlock()
    {
        _lockState = SequenceLockState.Unlocked;
    }
}
