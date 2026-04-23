using UnityEngine;
using UnityEngine.UI;
using TMPro;  // Si usas TextMeshPro

public class LockedDoor : MonoBehaviour
{
    [Header("Configuración")]
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.E;

    [Header("Animación")]
    public float openAngle = 90f;
    public float openSpeed = 3f;

    [Header("UI (opcional)")]
    public GameObject lockedMessageUI; // Un Text que diga "Necesitas una llave"

    private bool isOpen = false;
    private bool isUnlocked = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Camera playerCamera;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = transform.rotation * Quaternion.Euler(0f, 0f, openAngle);
        playerCamera = Camera.main;

        if (lockedMessageUI != null)
            lockedMessageUI.SetActive(false);
    }

    void Update()
    {
        // Animar
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            isOpen ? openRotation : closedRotation,
            Time.deltaTime * openSpeed
        );

        if (Input.GetKeyDown(interactKey))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            {
                if (hit.transform == transform)
                {
                    TryOpen();
                }
            }
        }

        // Ocultar mensaje tras 2 segundos (se gestiona con corrutina)
    }

    void TryOpen()
    {
        if (PlayerInventory.Instance.hasKey)
        {
            isOpen = true;
            isUnlocked = true;
            Debug.Log("Puerta abierta con la llave.");
        }
        else
        {
            Debug.Log("Necesitas una llave.");
            if (lockedMessageUI != null)
                StartCoroutine(ShowMessage());
        }
    }

    System.Collections.IEnumerator ShowMessage()
    {
        lockedMessageUI.SetActive(true);
        yield return new WaitForSeconds(2f);
        lockedMessageUI.SetActive(false);
    }
}