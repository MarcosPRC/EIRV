using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FootstepAudio : MonoBehaviour
{
    [Header("Clips de audio")]
    public AudioClip[] walkSteps;   // varios clips para variar
    public AudioClip[] runSteps;

    [Header("Configuración")]
    public float walkStepInterval = 0.5f;
    public float runStepInterval = 0.3f;

    private AudioSource audioSource;
    private Rigidbody rb;
    private float stepTimer = 0f;

    // Referencia al FPC para saber si está corriendo
    private FirstPersonController fpc;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f; // 2D
        audioSource.volume = 0.6f;

        rb = GetComponent<Rigidbody>();
        fpc = GetComponent<FirstPersonController>();
    }

    void Update()
    {
        // Velocidad horizontal
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        bool isMoving = flatVel.magnitude > 0.5f;

        // Detectar si está corriendo leyendo el input del shift
        // (fpc.sprintSpeed es público, lo usamos como referencia)
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && isMoving;

        if (isMoving)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayStep(isSprinting);
                stepTimer = isSprinting ? runStepInterval : walkStepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    void PlayStep(bool running)
    {
        AudioClip[] pool = running ? runSteps : walkSteps;
        if (pool == null || pool.Length == 0) return;

        AudioClip clip = pool[Random.Range(0, pool.Length)];
        audioSource.PlayOneShot(clip);
    }
}