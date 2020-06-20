using UnityEngine;

public class GameStateUIScript : MonoBehaviour
{
    [SerializeField]
    private GameObject _mainContainer;

    [SerializeField]
    private GameObject _gameWaitingUI;

    [SerializeField]
    private GameObject _gameStartedUI;

    [SerializeField]
    private GameObject _gameEndedUI;

    void Start()
    {
        SetWaitingPanelActive(false);
    }

    public void SetWaitingPanelActive(bool active)
    {
        _mainContainer.SetActive(active);
        _gameWaitingUI.SetActive(active);
    }
}
