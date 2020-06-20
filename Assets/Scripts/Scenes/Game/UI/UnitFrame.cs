using CardGame;
using CardGame.Constants;
using CardGame.Events;
using CardGame.Players;
using CardGame.Units.Base;
using NaughtyAttributes;
using UnityEngine;

public class UnitFrame : MonoBehaviour
{

    [SerializeField, ReadOnly]
    private bool _isPlayer = false;

    private Unit _unit;
    private Game _game;

    public Unit Unit => _unit;

    public void Ready(Game game, Unit unit)
    {
        _game = game;
        _unit = unit;
        _isPlayer = _unit is Player;

        if (!_isPlayer)
        {
            transform.localScale *= 0.85f;
        }

        _game.InterceptorService.RegisterInterceptor(InterceptorEvents.UnitKill, new Interceptor()
        {
            AllowRunInSimulation = false,
            Priority = int.MaxValue,
            After = (state) =>
            {
                Unit killedUnit = (Unit)state.Arguments.unit;
                if (killedUnit != _unit) return state;

                TaskScheduler.Instance.Queue(new UnityTask(delegate
                {
                    Destroy(gameObject, 1.5f);
                }));
                return state;
            }
        });
    }
}
