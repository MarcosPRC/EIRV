using UnityEngine;

public class AntidoteInteractable : MonoBehaviour
{
    [Header("Interacción")]
    public float interactDistance = 2.5f;
    public KeyCode interactKey = KeyCode.E;

    [Header("Animación")]
    public float floatSpeed = 1.2f;
    public float floatHeight = 0.1f;
    public float rotateSpeed = 60f;

    private Camera playerCamera;
    private Vector3 startPos;
    private bool used = false;

    void Start()
    {
        playerCamera = Camera.main;
        startPos = transform.position;
    }

    void Update()
    {
        if (used) return;

        // Flotación y rotación igual que la llave y las cartas
        float y = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPos.x, y, startPos.z);
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);

        if (Input.GetKeyDown(interactKey))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            {
                if (hit.transform == transform)
                {
                    UseAntidote();
                }
            }
        }
    }

    void UseAntidote()
    {
        used = true;
        DiseaseManager.Instance.CurePlayer();
        Destroy(gameObject);
    }
}