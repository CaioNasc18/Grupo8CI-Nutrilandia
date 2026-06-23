using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VectorGraphics;
using UnityEngine.SceneManagement;

public class PintarRio : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [Header("Pintura")]
    public RawImage rawImageRio;
    public Color corAzul = new Color(0.25f, 0.55f, 1f, 1f);
    public int tamanhoBrush = 18;

    [Header("Progresso")]
    [Range(0f, 1f)]
    public float percentagemVitoria = 0.7f;
    public Slider sliderProgresso; // opcional

    private Texture2D textura;
    private int pixelsPintados = 0;
    private int totalPixelsRio = 0; // ← agora conta só pixels do rio
    private bool jogoTerminado = false;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = rawImageRio.GetComponent<RectTransform>();

        Texture2D original = (Texture2D)rawImageRio.texture;
        textura = new Texture2D(original.width, original.height, TextureFormat.RGBA32, false);
        textura.SetPixels(original.GetPixels());
        textura.Apply();
        rawImageRio.texture = textura;

        // Conta apenas pixels que são do rio
        ContarPixelsRio();
    }

    void ContarPixelsRio()
    {
        for (int x = 0; x < textura.width; x++)
            for (int y = 0; y < textura.height; y++)
                if (EhRio(textura.GetPixel(x, y)))
                    totalPixelsRio++;

        Debug.Log($"Total de pixels do rio: {totalPixelsRio}");
    }

    public void OnPointerDown(PointerEventData eventData) => Pintar(eventData.position);
    public void OnDrag(PointerEventData eventData) => Pintar(eventData.position);

    void Pintar(Vector2 posicaoEcra)
    {
        if (jogoTerminado) return;

        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, posicaoEcra, null, out localPoint)) return;

        Rect rect = rectTransform.rect;
        float u = (localPoint.x - rect.x) / rect.width;
        float v = (localPoint.y - rect.y) / rect.height;

        if (u < 0 || u > 1 || v < 0 || v > 1) return;

        int px = (int)(u * textura.width);
        int py = (int)(v * textura.height);

        AplicarBrush(px, py);
        textura.Apply();
        AtualizarProgresso();
    }

    void AplicarBrush(int cx, int cy)
    {
        int r = tamanhoBrush;
        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y > r * r) continue;

                int px = cx + x;
                int py = cy + y;

                if (px < 0 || px >= textura.width || py < 0 || py >= textura.height) continue;

                Color atual = textura.GetPixel(px, py);

                // ✅ Só pinta se for pixel do rio E ainda não estiver pintado
                if (EhRio(atual) && !JaPintado(atual))
                {
                    pixelsPintados++;
                    textura.SetPixel(px, py, corAzul);
                }
            }
        }
    }

    bool EhRio(Color c)
    {
        return Mathf.Abs(c.r - 0.55f) < 0.15f &&
               Mathf.Abs(c.g - 0.79f) < 0.15f &&
               Mathf.Abs(c.b - 0.76f) < 0.15f;
    }

    bool JaPintado(Color c)
    {
        return Mathf.Abs(c.r - corAzul.r) < 0.1f &&
               Mathf.Abs(c.g - corAzul.g) < 0.1f &&
               Mathf.Abs(c.b - corAzul.b) < 0.1f;
    }

    void AtualizarProgresso()
    {
        if (totalPixelsRio == 0) return;

        float progresso = (float)pixelsPintados / totalPixelsRio;

        if (sliderProgresso != null)
            sliderProgresso.value = progresso;

        Debug.Log($"Progresso: {progresso * 100f:F1}%");

        if (progresso >= percentagemVitoria && !jogoTerminado)
        {
            jogoTerminado = true;
            SceneManager.LoadScene("FimLimpezaRio");
        }
    }
}