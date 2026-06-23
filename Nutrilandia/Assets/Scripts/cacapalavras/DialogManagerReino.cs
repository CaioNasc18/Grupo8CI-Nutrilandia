using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManagerReino : MonoBehaviour
{
    [Header("NPC")]
    [SerializeField] private RectTransform npcRect;
    [SerializeField] private CanvasGroup npcCanvasGroup;

    [Header("Dialog Box")]
    [SerializeField] private CanvasGroup dialogBoxGroup;
    [SerializeField] private RectTransform dialogBoxRect;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private TextMeshProUGUI speakerName;
    [SerializeField] private GameObject continueArrow;

    [Header("Overlay")]
    [SerializeField] private Image overlayImage;

    [Header("Configurações de Animação")]
    [SerializeField] private float overlayAlphaTarget = 0.55f;
    [SerializeField] private float fadeDuration = 0.4f;
    [SerializeField] private float npcSlideDuration = 0.55f;
    [SerializeField] private float npcOffscreenOffsetY = -200f;
    [SerializeField] private float dialogPopDuration = 0.35f;

    [Header("Typewriter")]
    [SerializeField] private float charDelay = 0.04f;

    [Header("Input")]
    [SerializeField] private GameObject clickPanel;

    [Header("Diálogos")]
    [SerializeField] private DialogLineReino[] lines;

    [Header("Configuração")]
    [SerializeField] private bool startOnLoad = false;
    [SerializeField] private string nextSceneName = ""; // deixa vazio se não quer mudar de cena

    // --- estado interno ---
    private int currentLine = 0;
    private bool isTyping = false;
    private bool dialogActive = false;
    private Coroutine typewriterCoroutine;
    private Vector2 npcStartPos;

    public bool IsDialogActive => dialogActive;

    // -------------------------------------------------------

    private void Awake()
    {
        npcStartPos = npcRect.anchoredPosition;

        SetAlpha(overlayImage, 0f);
        SetCanvasGroupAlpha(npcCanvasGroup, 0f);
        SetCanvasGroupAlpha(dialogBoxGroup, 0f);
        dialogBoxRect.localScale = Vector3.one * 0.85f;
        npcRect.anchoredPosition = new Vector2(npcStartPos.x, npcStartPos.y + npcOffscreenOffsetY);
        continueArrow.SetActive(false);
    }

    private void Start()
    {
        if (startOnLoad)
            StartDialog();
    }

    // -------------------------------------------------------
    // Público
    // -------------------------------------------------------

    public void StartDialog()
    {
        if (dialogActive) return;
        dialogActive = true;
        currentLine = 0;
        StartCoroutine(PlayIntroSequence());
    }

    public void OnScreenClick()
    {
        if (!dialogActive) return;

        if (isTyping)
        {
            SkipTypewriter();
            return;
        }

        AdvanceDialog();
    }

    // Injetado pelo PlateGameManager para falas de erro/vitória
    public void SetSingleLine(string speaker, string text)
    {
        lines = new DialogLineReino[]
        {
            new DialogLineReino { speakerName = speaker, text = text }
        };
    }

    // -------------------------------------------------------
    // Sequência de entrada
    // -------------------------------------------------------

    private IEnumerator PlayIntroSequence()
    {
        clickPanel.SetActive(true);
        yield return StartCoroutine(FadeImage(overlayImage, 0f, overlayAlphaTarget, fadeDuration));
        yield return StartCoroutine(SlideNpcUp());
        yield return StartCoroutine(PopInDialogBox());
        ShowLine(currentLine);
    }

    // -------------------------------------------------------
    // Animações
    // -------------------------------------------------------

    private IEnumerator FadeImage(Image img, float from, float to, float duration)
    {
        float elapsed = 0f;
        Color c = img.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, elapsed / duration);
            img.color = c;
            yield return null;
        }
        c.a = to;
        img.color = c;
    }

    private IEnumerator SlideNpcUp()
    {
        Vector2 startPos = new Vector2(npcStartPos.x, npcStartPos.y + npcOffscreenOffsetY);
        Vector2 endPos = npcStartPos;
        float elapsed = 0f;
        SetCanvasGroupAlpha(npcCanvasGroup, 1f);

        while (elapsed < npcSlideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / npcSlideDuration;
            npcRect.anchoredPosition = Vector2.LerpUnclamped(startPos, endPos, SpringEase(t));
            yield return null;
        }
        npcRect.anchoredPosition = endPos;
    }

    private IEnumerator PopInDialogBox()
    {
        float elapsed = 0f;
        SetCanvasGroupAlpha(dialogBoxGroup, 1f);

        while (elapsed < dialogPopDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dialogPopDuration;
            dialogBoxRect.localScale = Vector3.LerpUnclamped(Vector3.one * 0.85f, Vector3.one, SpringEase(t));
            yield return null;
        }
        dialogBoxRect.localScale = Vector3.one;
    }

    private float SpringEase(float t)
    {
        t = Mathf.Clamp01(t);
        return 1f - Mathf.Pow(2f, -10f * t) * Mathf.Cos(t * Mathf.PI * 2.2f);
    }

    // -------------------------------------------------------
    // Diálogo e typewriter
    // -------------------------------------------------------

    private void ShowLine(int index)
    {
        if (index >= lines.Length)
        {
            StartCoroutine(PlayOutroSequence());
            return;
        }

        continueArrow.SetActive(false);
        speakerName.text = lines[index].speakerName;

        if (typewriterCoroutine != null)
            StopCoroutine(typewriterCoroutine);

        typewriterCoroutine = StartCoroutine(Typewriter(lines[index].text));
    }

    private IEnumerator Typewriter(string fullText)
    {
        isTyping = true;
        dialogText.text = "";

        foreach (char c in fullText)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(charDelay);
        }

        isTyping = false;
        continueArrow.SetActive(true);
    }

    private void SkipTypewriter()
    {
        if (typewriterCoroutine != null)
            StopCoroutine(typewriterCoroutine);

        dialogText.text = lines[currentLine].text;
        isTyping = false;
        continueArrow.SetActive(true);
    }

    private void AdvanceDialog()
    {
        currentLine++;

        if (currentLine >= lines.Length)
            StartCoroutine(PlayOutroSequence());
        else
            ShowLine(currentLine);
    }

    // -------------------------------------------------------
    // Saída
    // -------------------------------------------------------

    private IEnumerator PlayOutroSequence()
    {
        clickPanel.SetActive(false);
        dialogActive = false;
        continueArrow.SetActive(false);

        yield return StartCoroutine(FadeCanvasGroup(dialogBoxGroup, 1f, 0f, 0.25f));
        yield return StartCoroutine(SlideNpcDown());
        yield return StartCoroutine(FadeImage(overlayImage, overlayAlphaTarget, 0f, fadeDuration));

        // Só muda de cena se nextSceneName estiver preenchido
        if (!string.IsNullOrEmpty(nextSceneName))
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator SlideNpcDown()
    {
        Vector2 startPos = npcRect.anchoredPosition;
        Vector2 endPos = new Vector2(npcStartPos.x, npcStartPos.y + npcOffscreenOffsetY);
        float elapsed = 0f;
        float duration = 0.4f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            npcRect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }
        SetCanvasGroupAlpha(npcCanvasGroup, 0f);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        cg.alpha = to;
    }

    // -------------------------------------------------------
    // Helpers
    // -------------------------------------------------------

    private void SetAlpha(Image img, float a)
    {
        Color c = img.color;
        c.a = a;
        img.color = c;
    }

    private void SetCanvasGroupAlpha(CanvasGroup cg, float a)
    {
        cg.alpha = a;
        cg.interactable = a > 0f;
        cg.blocksRaycasts = a > 0f;
    }
}

[System.Serializable]
public class DialogLineReino
{
    public string speakerName = "Capitão Cenoura";
    [TextArea(2, 4)]
    public string text;
}
