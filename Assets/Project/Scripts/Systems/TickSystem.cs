using UnityEngine;


public class TickSystem : MonoBehaviour


{

    public static TickSystem I { get; private set; }

    [Tooltip("Ticks por segundo (10 recomendado).")]
    public int ticksPerSecond = 10;

    [Header("Dev")]
    public float devMultiplier = 1f; // 1 normal, 5 para pruebas

    float _acc;      // acumulador en segundos
    float _step;     // intervalo por tick

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);
        _step = 1f / Mathf.Max(1, ticksPerSecond);
    }

    void Update()
    {
        _acc += Time.unscaledDeltaTime;  // no depende del timescale
        while (_acc >= _step)
        {
            if (GameState.I != null)
            GameState.I.Tick(_step * devMultiplier);        
            _acc -= _step;
        }
    }
}
