using UnityEngine;
using UnityEngine.EventSystems;

public class MenuBotaoEfeitos : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Efeito Hover (Multiplicador)")]
    [Tooltip("1.08 significa que o botão vai crescer 8% do seu tamanho original")]
    public float multiplicadorTamanho = 1.08f;
    public float velocidadeHover = 12f;

    [Header("Animação de Entrada")]
    public float distanciaSubida = 50f;
    public float duracaoEntrada = 0.5f;

    private Vector3 tamanhoOriginal;
    private Vector3 tamanhoAlvo;
    private CanvasGroup canvasGroup;
    
    private Vector3 posicaoFinal;
    private Vector3 posicaoInicial;
    private float tempoDecorridoEntrada = 0f;
    private bool aFazerEntrada = true;

    void Awake()
    {
        posicaoFinal = transform.localPosition;
        posicaoInicial = posicaoFinal - new Vector3(0, distanciaSubida, 0);
        transform.localPosition = posicaoInicial;

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0f;
    }

    void Start()
    {
        // Guarda o tamanho real que o botão tem na cena
        tamanhoOriginal = transform.localScale;
        tamanhoAlvo = tamanhoOriginal;
    }

    void Update()
    {
        if (aFazerEntrada)
        {
            tempoDecorridoEntrada += Time.deltaTime;
            float progresso = tempoDecorridoEntrada / duracaoEntrada;
            float curvaSuave = Mathf.SmoothStep(0f, 1f, progresso);

            transform.localPosition = Vector3.Lerp(posicaoInicial, posicaoFinal, curvaSuave);
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, curvaSuave);

            if (progresso >= 1f)
            {
                transform.localPosition = posicaoFinal;
                canvasGroup.alpha = 1f;
                aFazerEntrada = false;
            }
            return;
        }

        transform.localScale = Vector3.Lerp(transform.localScale, tamanhoAlvo, Time.deltaTime * velocidadeHover);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (aFazerEntrada) return;
        // Calcula o tamanho alvo multiplicando o tamanho original pelo fator de crescimento
        tamanhoAlvo = tamanhoOriginal * multiplicadorTamanho;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tamanhoAlvo = tamanhoOriginal;
    }
}
