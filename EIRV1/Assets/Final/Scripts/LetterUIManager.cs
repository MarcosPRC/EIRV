using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LetterUIManager : MonoBehaviour
{
    public static LetterUIManager Instance;

    [Header("Panel carta")]
    public GameObject letterPanel;      // Panel que contiene todo
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public CanvasGroup canvasGroup;      // Para fade

    [Header("Animación")]
    public float fadeInDuration = 0.4f;
    public float fadeOutDuration = 0.3f;

    [Header("Control")]
    public KeyCode closeKey = KeyCode.Mouse0; // Click izquierdo

    private bool isShowing = false;
    private FirstPersonController fpc; // Para bloquear movimiento

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        letterPanel.SetActive(false);
    }

    void Start()
    {
        fpc = FindFirstObjectByType<FirstPersonController>();
    }

    void Update()
    {
        if (isShowing && Input.GetKeyDown(closeKey))
            StartCoroutine(HideLetter());
    }

    public void ShowLetter(string title, string body)
    {
        StartCoroutine(ShowLetterCoroutine(title, body));
    }

    IEnumerator ShowLetterCoroutine(string title, string body)
    {
        isShowing = true;

        // Bloquear jugador
        if (fpc != null)
        {
            fpc.cameraCanMove = false;
            fpc.playerCanMove = false;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        titleText.text = title;
        bodyText.text = "";
        canvasGroup.alpha = 0f;
        letterPanel.SetActive(true);

        // Fade in del panel
        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / fadeInDuration);
            yield return null;
        }

        // Escribir el texto letra a letra
        yield return StartCoroutine(TypeText(bodyText, body));
    }

    IEnumerator TypeText(TextMeshProUGUI target, string text)
    {
        target.text = "";
        foreach (char c in text)
        {
            target.text += c;
            yield return new WaitForSeconds(0.03f);
        }
    }

    IEnumerator HideLetter()
    {
        float t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(t / fadeOutDuration);
            yield return null;
        }

        letterPanel.SetActive(false);
        isShowing = false;

        // Devolver control al jugador
        if (fpc != null)
        {
            fpc.cameraCanMove = true;
            fpc.playerCanMove = true;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}