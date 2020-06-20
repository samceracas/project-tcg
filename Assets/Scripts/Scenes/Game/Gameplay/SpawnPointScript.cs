using UnityEngine;

public class SpawnPointScript : MonoBehaviour
{
    [SerializeField]
    private bool _isOccupied = false;

    public bool IsOccupied { get => _isOccupied; }

    public void DelayedSetUnOccupied(float speed)
    {
        Invoke("SetUnOccupied", speed);
    }

    public void SetOccupied()
    {
        _isOccupied = true;
    }

    public void SetUnOccupied()
    {
        _isOccupied = false;
    }
}
