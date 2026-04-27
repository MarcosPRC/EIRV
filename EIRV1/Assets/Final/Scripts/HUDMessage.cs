using System.Collections;
using UnityEngine;
using TMPro;

public class HUDMessage : MonoBehaviour
{
    public static HUDMessage Instance;

    public TextMeshProUGUI messageText;
    public float displayDuration = 3f;
    public float fadeSpeed = 2f;

    private Coroutine currentCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        messageText.alpha = 0f;
    }

    public void ShowMessage(string msg)
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(DisplayMessage(msg));
    }

    IEnumerator DisplayMessage(string msg)
    {
        messageText.text = msg;
        messageText.alpha = 1f;

        yield return new WaitForSeconds(displayDuration);

        // Fade out
        while (messageText.alpha > 0f)
        {
            messageText.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }
}