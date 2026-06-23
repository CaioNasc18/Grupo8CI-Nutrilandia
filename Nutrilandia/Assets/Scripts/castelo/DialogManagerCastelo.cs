using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum NpcSideCastelo { Esquerda, Direita }

[System.Serializable]
public class DialogLineCastelo
{
    public string speakerName = "Capitão Cenoura";
    [TextArea(2, 4)] public string text;
    public NpcSideCastelo npcQueFala = NpcSideCastelo.Esquerda; // Escolhe qual NPC fala nesta linha
}

public class DialogManagerCastelo : MonoBehaviour
{
    [Header("NPC Esquerda")]
    [SerializeField] private RectTransform npcLeftRect;
    [SerializeField] private CanvasGroup npcLeftCanvasGroup;

    [Header("NPC Direita")]
    [SerializeField] private RectTransform npcRightRect;
    [SerializeField] private CanvasGroup npcRightCanvasGroup;

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
    [SerializeField] private DialogLineCastelo[] lines;

    [Header("Configuração")]
    [SerializeField] private bool startOnLoad = false;
    [SerializeField] private string nextSceneName = "";

    private int currentLine = 0;
    private bool isTyping = false;
    private bool dialogActive = false;
    private Coroutine typewriterCoroutine;

    // Posições originais guardadas no Awake
    private Vector2 npcLeftStartPos;
    private Vector2 npcRightStartPos;

    public bool IsDialogActive => dialogActive;

    private void Awake()
    {
        // Guarda as posições iniciais configuradas no Canvas
        if (npcLeftRect != null) npcLeftStartPos = npcLeftRect.anchoredPosition;
        if (npcRightRect != null) npcRightStartPos = npcRightRect.anchoredPosition;

        // Reset visual do cenário
        SetAlpha(overlayImage, 0f);
        SetCanvasGroupAlpha(npcLeftCanvasGroup, 0f);
        SetCanvasGroupAlpha(npcRightCanvasGroup, 0f);
        SetCanvasGroupAlpha(dialogBoxGroup, 0f);

        dialogBoxRect.localScale = Vector3.one * 0.85f;
        continueArrow.SetActive(false);

        // Esconde os NPCs abaixo do ecrã
        if (npcLeftRect != null) npcLeftRect.anchoredPosition = new Vector2(npcLeftStartPos.x, npcLeftStartPos.y + npcOffscreenOffsetY);
        if (npcRightRect != null) npcRightRect.anchoredPosition = new Vector2(npcRightStartPos.x, npcRightStartPos.y + npcOffscreenOffsetY);
    }

    private void Start()
    {
        if (startOnLoad)
            StartDialog();
    }

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

    public void SetSingleLine(string speaker, string text, NpcSideCastelo side = NpcSideCastelo.Esquerda)
    {
        lines = new DialogLineCastelo[]
        {
            new DialogLineCastelo { speakerName = speaker, text = text, npcQueFala = side }
        };
    }

    private IEnumerator PlayIntroSequence()
    {
        clickPanel.SetActive(true);
        yield return StartCoroutine(FadeImage(overlayImage, 0f, overlayAlphaTarget, fadeDuration));

        // Entram os dois NPCs ao mesmo tempo na introdução
        StartCoroutine(SlideNpcUp(npcLeftRect, npcLeftCanvasGroup, npcLeftStartPos));
        yield return StartCoroutine(SlideNpcUp(npcRightRect, npcRightCanvasGroup, npcRightStartPos));

        yield return StartCoroutine(PopInDialogBox());
        ShowLine(currentLine);
    }

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

    private IEnumerator SlideNpcUp(RectTransform rect, CanvasGroup cg, Vector2 startPos)
    {
        if (rect == null || cg == null) yield break;

        Vector2 spawnPos = new Vector2(startPos.x, startPos.y + npcOffscreenOffsetY);
        float elapsed = 0f;
        SetCanvasGroupAlpha(cg, 1f);

        while (elapsed < npcSlideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / npcSlideDuration;
            rect.anchoredPosition = Vector2.LerpUnclamped(spawnPos, startPos, SpringEase(t));
            yield return null;
        }
        rect.anchoredPosition = startPos;
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

    private void ShowLine(int index)
    {
        if (index >= lines.Length)
        {
            StartCoroutine(PlayOutroSequence());
            return;
        }

        continueArrow.SetActive(false);
        speakerName.text = lines[index].speakerName;

        // Destaca visualmente o NPC que está a falar (escurece o outro)
        DestaqueNpcAtivo(lines[index].npcQueFala);

        if (typewriterCoroutine != null)
            StopCoroutine(typewriterCoroutine);

        typewriterCoroutine = StartCoroutine(Typewriter(lines[index].text));
    }

    private void DestaqueNpcAtivo(NpcSideCastelo ladoAtivo)
    {
        // 1.0f para opacidade total (destacado), 0.5f para meio opaco (escondido/fundo)
        if (npcLeftCanvasGroup != null) npcLeftCanvasGroup.alpha = (ladoAtivo == NpcSideCastelo.Esquerda) ? 1.0f : 0.5f;
        if (npcRightCanvasGroup != null) npcRightCanvasGroup.alpha = (ladoAtivo == NpcSideCastelo.Direita) ? 1.0f : 0.5f;
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

    private IEnumerator PlayOutroSequence()
    {
        clickPanel.SetActive(false);
        dialogActive = false;
        continueArrow.SetActive(false);

        yield return StartCoroutine(FadeCanvasGroup(dialogBoxGroup, 1f, 0f, 0.25f));

        // Retira os dois NPCs ao mesmo tempo
        StartCoroutine(SlideNpcDown(npcLeftRect, npcLeftCanvasGroup, npcLeftStartPos));
        yield return StartCoroutine(SlideNpcDown(npcRightRect, npcRightCanvasGroup, npcRightStartPos));

        yield return StartCoroutine(FadeImage(overlayImage, overlayAlphaTarget, 0f, fadeDuration));

        if (!string.IsNullOrEmpty(nextSceneName))
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator SlideNpcDown(RectTransform rect, CanvasGroup cg, Vector2 startPos)
    {
        if (rect == null || cg == null) yield break;

        Vector2 currentPos = rect.anchoredPosition;
        Vector2 endPos = new Vector2(startPos.x, startPos.y + npcOffscreenOffsetY);
        float elapsed = 0f;
        float duration = 0.4f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            rect.anchoredPosition = Vector2.Lerp(currentPos, endPos, t);
            yield return null;
        }
        SetCanvasGroupAlpha(cg, 0f);
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
        cg.interactable = to > 0f;
        cg.blocksRaycasts = to > 0f;
    }

    private void SetAlpha(Image img, float a)
    {
        if (img != null)
        {
            Color c = img.color;
            c.a = a;
            img.color = c;
        }
    }

    private void SetCanvasGroupAlpha(CanvasGroup cg, float a)
    {
        if (cg != null)
        {
            cg.alpha = a;
            cg.interactable = a > 0f;
            cg.blocksRaycasts = a > 0f;
        }
    }
}
