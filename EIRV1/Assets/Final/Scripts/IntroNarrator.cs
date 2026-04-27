using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class IntroNarrator : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI narratorText;
    public CanvasGroup canvasGroup;

    [Header("Escena destino")]
    public string gameSceneName = "Hospital";

    [Header("Líneas de diálogo")]
    [TextArea(2, 5)]
    public string[] lines = new string[]
    {
        "Madrid, 3 de marzo de 2024.",
        "Me llamo Álex. Soy médico.",
        "Hace tres días entré a este hospital a investigar los rumores sobre un brote.",
        "Fui un estúpido. Me contagié.",
        "Tengo 10 minutos antes de que el virus haga efecto irreversible.",
        "Sé que hay un antídoto aquí dentro. Tengo que encontrarlo.",
        "Pulsa cualquier tecla para comenzar..."
    };

    [Header("Tiempos")]
    public float typingSpeed = 0.04f;
    public float linePauseDuration = 1.8f;
    public float fadeInDuration = 0.5f;
    public float fadeOutDuration = 1.2f;

    void Start()
    {
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        canvasGroup.alpha = 0f;

        // Fade in inicial
        yield return Fade(0f, 1f, fadeInDuration);

        for (int i = 0; i < lines.Length; i++)
        {
            narratorText.text = "";

            // Última línea: esperar input
            if (i == lines.Length - 1)
            {
                yield return StartCoroutine(TypeLine(lines[i]));
                yield return new WaitUntil(() => Input.anyKeyDown);
                break;
            }
            else
            {
                yield return StartCoroutine(TypeLine(lines[i]));
                yield return new WaitForSeconds(linePauseDuration);
            }
        }

        // Fade out y cargar juego
        yield return Fade(1f, 0f, fadeOutDuration);
        SceneManager.LoadScene(gameSceneName);
    }

    IEnumerator TypeLine(string line)
    {
        narratorText.text = "";
        foreach (char c in line)
        {
            narratorText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}