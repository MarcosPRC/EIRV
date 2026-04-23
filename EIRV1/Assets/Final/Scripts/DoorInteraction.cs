using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    [Header("Configuración")]
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.E;

    [Header("Animación")]
    public float openAngle = -90f;
    public float openSpeed = 3f;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Camera playerCamera;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = transform.rotation * Quaternion.Euler(0f, 0f, openAngle);
        playerCamera = Camera.main;
    }

    void Update()
    {
        // Rotar suavemente
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            isOpen ? openRotation : closedRotation,
            Time.deltaTime * openSpeed
        );

        // Detectar input
        if (Input.GetKeyDown(interactKey))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            {
                if (hit.transform == transform)
                {
                    isOpen = !isOpen;
                }
            }
        }
    }

    // Mostrar rango en editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}