using UnityEngine;


public class TickSystem : MonoBehaviour


{
    private const float MaxAcceptedFrameDeltaSeconds = 0.5f;
    private const int MaxTicksPerFrame = 20;

    public static TickSystem I { get; private set; }

    [Tooltip("Ticks por segundo (10 recomendado).")]
    public int ticksPerSecond = 10;

    [Header("Dev (compatibilidad serializada)")]
    [Tooltip("Campo heredado. QaRuntimeService es la única autoridad activa.")]
    public float devMultiplier = 1f;

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
        devMultiplier = 1f;
    }

    void Update()
    {
        AdvanceFrame(Time.unscaledDeltaTime);
    }

    private void AdvanceFrame(float unscaledDeltaTime)
    {
        if (float.IsNaN(unscaledDeltaTime) ||
            float.IsInfinity(unscaledDeltaTime) || unscaledDeltaTime < 0f)
        {
            return;
        }

        // Android puede entregar un delta muy grande al volver del segundo plano.
        // El progreso ausente se aplica como offline desde SaveService; no debe
        // convertirse otra vez en miles de ticks dentro de un solo fotograma.
        _acc += Mathf.Min(unscaledDeltaTime, MaxAcceptedFrameDeltaSeconds);

        int processedTicks = 0;
        while (_acc >= _step && processedTicks < MaxTicksPerFrame)
        {
            if (GameState.I != null)
            {
                double simulatedSeconds =
                    QaRuntimeService.ScaleOnlineSeconds(_step);
                GameState.I.Tick(simulatedSeconds);
            }
            _acc -= _step;
            processedTicks++;
        }

        // Protección adicional frente a un acumulador heredado de una pausa o
        // de un bloqueo prolongado del hilo principal.
        if (_acc >= _step)
            _acc = 0f;
    }

    public void ResetAccumulator()
    {
        _acc = 0f;
    }

    private void OnApplicationPause(bool pause)
    {
        ResetAccumulator();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
            ResetAccumulator();
    }
}
