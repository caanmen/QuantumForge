using UnityEngine;

public class TickSystem : MonoBehaviour
{
    [Tooltip("Ticks por segundo (10 recomendado).")]
    public int ticksPerSecond = 10;

    float _acc;      // acumulador en segundos
    float _step;     // intervalo por tick

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _step = 1f / Mathf.Max(1, ticksPerSecond);
    }

    void Update()
    {
        _acc += Time.unscaledDeltaTime;  // no depende del timescale
        while (_acc >= _step)
        {
            if (GameState.I != null)
                GameState.I.Tick(_step);
            _acc -= _step;
        }
    }
}
