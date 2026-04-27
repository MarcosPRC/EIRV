using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [Header("Configuración")]
    public float interactDistance = 2.5f;
    public KeyCode interactKey = KeyCode.E;

    [Header("Visual")]
    public float floatSpeed = 1.5f;
    public float floatHeight = 0.15f;
    public float rotateSpeed = 90f;

    private Camera playerCamera;
    private Vector3 startPosition;

    void Start()
    {
        playerCamera = Camera.main;
        startPosition = transform.position;
    }

    void Update()
    {
        // Animación de flotación y rotación
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);

        // Recoger con E
        if (Input.GetKeyDown(interactKey))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            {
                if (hit.transform == transform)
                {
                    PickUp();
                }
            }
        }
    }

    void PickUp()
    {
        PlayerInventory.Instance.hasKey = true;
        Debug.Log("¡Llave recogida!");
        // Aquí puedes reproducir un sonido de recogida
        // AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        //HUDMessage.Instance.ShowMessage("¡Llave encontrada!");
        Destroy(gameObject);
    }
}