using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DiseaseManager : MonoBehaviour
{
    public static DiseaseManager Instance;

    [Header("Timer")]
    public float totalTime = 600f; // 10 minutos
    private float timeRemaining;
    private bool isDead = false;
    private bool isCured = false;

    [Header("Postprocesado (URP Global Volume)")]
    public Volume globalVolume;
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;
    private LensDistortion lensDistortion;

    [Header("Audio")]
    public AudioSource ambientAudio;   // Sonido ambiente del hospital
    public AudioSource heartbeatAudio; // Latido que sube con la enfermedad
    public AnimationCurve volumeCurve; // Curva de bajada de volumen ambiente

    [Header("UI")]
    public Image bloodOverlay;    // Imagen roja semitransparente fullscreen
    public GameObject deathScreen;
    public GameObject curedScreen;

    [Header("Escenas")]
    public string menuSceneName = "MainMenu";

    // Estado interno
    private float diseaseProgress = 0f; // 0 = sano, 1 = muerto

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        timeRemaining = totalTime;

        // Obtener efectos del Volume
        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out vignette);
            globalVolume.profile.TryGet(out colorAdjustments);
            globalVolume.profile.TryGet(out lensDistortion);
        }

        // Inicializar UI
        if (bloodOverlay != null)
        {
            Color c = bloodOverlay.color;
            c.a = 0f;
            bloodOverlay.color = c;
        }
        if (deathScreen != null) deathScreen.SetActive(false);
        if (curedScreen != null) curedScreen.SetActive(false);
    }

    void Update()
    {
        if (isDead || isCured) return;

        timeRemaining -= Time.deltaTime;
        diseaseProgress = 1f - Mathf.Clamp01(timeRemaining / totalTime);

        ApplyDiseaseEffects(diseaseProgress);

        if (timeRemaining <= 0f)
        {
            Die();
        }
    }

    void ApplyDiseaseEffects(float t)
    {
        // --- VIGNETTE: oscurecer bordes ---
        if (vignette != null)
        {
            vignette.intensity.value = Mathf.Lerp(0.15f, 0.85f, t);
            // Color rojo en la viñeta
            vignette.color.value = Color.Lerp(Color.black, new Color(0.5f, 0f, 0f), t);
        }

        // --- COLOR ADJUSTMENTS: tinte rojo + desaturación ---
        if (colorAdjustments != null)
        {
            // Bajar saturación progresivamente
            colorAdjustments.saturation.value = Mathf.Lerp(0f, -60f, t);
            // Temperatura fría/roja
            colorAdjustments.colorFilter.value = Color.Lerp(
                Color.white,
                new Color(1f, 0.3f, 0.3f),
                t * 0.7f
            );
        }

        // --- LENS DISTORTION: distorsión al final ---
        if (lensDistortion != null)
        {
            lensDistortion.intensity.value = Mathf.Lerp(0f, -0.35f, Mathf.Pow(t, 2f));
        }

        // --- OVERLAY ROJO: sangre en pantalla ---
        if (bloodOverlay != null)
        {
            Color c = bloodOverlay.color;
            // Parpadeo sutil que aumenta con la enfermedad
            float pulse = 0.5f + 0.5f * Mathf.Sin(Time.time * (1f + t * 4f));
            c.a = Mathf.Lerp(0f, 0.4f, t) * (0.7f + 0.3f * pulse);
            bloodOverlay.color = c;
        }

        // --- AUDIO AMBIENTE: bajar volumen ---
        if (ambientAudio != null)
        {
            ambientAudio.volume = Mathf.Lerp(1f, 0f, t);
        }

        // --- LATIDO: sube con enfermedad ---
        if (heartbeatAudio != null)
        {
            heartbeatAudio.volume = Mathf.Lerp(0f, 1f, t);
            // Acelerar pitch con la enfermedad
            heartbeatAudio.pitch = Mathf.Lerp(0.8f, 1.6f, t);
        }
    }

    public void CurePlayer()
    {
        if (isCured) return;
        StartCoroutine(CureCoroutine());
    }

    System.Collections.IEnumerator CureCoroutine()
    {
        isCured = true;
        float cureDuration = 4f;
        float elapsed = 0f;

        while (elapsed < cureDuration)
        {
            elapsed += Time.deltaTime;
            float t = 1f - (elapsed / cureDuration); // De enfermo a sano

            ApplyDiseaseEffects(diseaseProgress * t);
            yield return null;
        }

        // Limpiar todos los efectos
        ApplyDiseaseEffects(0f);

        if (curedScreen != null) curedScreen.SetActive(true);

        // Esperar y volver al menú o mostrar créditos
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(menuSceneName);
    }

    void Die()
    {
        isDead = true;
        if (deathScreen != null) deathScreen.SetActive(true);
        StartCoroutine(LoadMenuAfterDeath());
    }

    System.Collections.IEnumerator LoadMenuAfterDeath()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(menuSceneName);
    }

    // Llamado desde UI o debug
    public float GetDiseaseProgress() => diseaseProgress;
    public float GetTimeRemaining() => timeRemaining;
}