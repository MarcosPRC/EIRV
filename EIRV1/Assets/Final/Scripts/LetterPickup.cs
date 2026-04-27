using UnityEngine;

public class LetterPickup : MonoBehaviour
{
    [Header("Contenido")]
    [TextArea(4, 12)]
    public string letterText = "Escribe aquí el texto de la carta...";

    public string letterTitle = "Nota";

    [Header("Animación")]
    public float floatSpeed = 1.2f;
    public float floatHeight = 0.1f;
    public float rotateSpeed = 60f;

    [Header("Interacción")]
    public float interactDistance = 2.5f;
    public KeyCode interactKey = KeyCode.E;

    private Camera playerCamera;
    private Vector3 startPos;
    private bool pickedUp = false;

    void Start()
    {
        playerCamera = Camera.main;
        startPos = transform.position;
    }

    void Update()
    {
        if (pickedUp) return;

        // Flotación
        float y = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPos.x, y, startPos.z);
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);

        // Interacción
        if (Input.GetKeyDown(interactKey))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            {
                if (hit.transform == transform)
                    Collect();
            }
        }
    }

    void Collect()
    {
        pickedUp = true;
        LetterUIManager.Instance.ShowLetter(letterTitle, letterText);
        Destroy(gameObject);
    }
}