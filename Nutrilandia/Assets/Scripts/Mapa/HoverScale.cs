using UnityEngine;
using UnityEngine.EventSystems; // Biblioteca necessária para eventos de UI

public class HoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configurações de Tamanho")]
    public Vector3 tamanhoDesejado = new Vector3(1.2f, 1.2f, 1.2f); // Define para quanto o objeto cresce (1.2f = 20% maior)
    public float velocidade = 10f; // Velocidade da transição

    private Vector3 tamanhoOriginal;
    private Vector3 tamanhoAlvo;

    void Start()
    {
        // Salva o tamanho original do objeto/botão
        tamanhoOriginal = transform.localScale;
        tamanhoAlvo = tamanhoOriginal;
    }

    void Update()
    {
        // Transição suave do tamanho atual para o tamanho alvo
        transform.localScale = Vector3.Lerp(transform.localScale, tamanhoAlvo, Time.deltaTime * velocidade);
    }

    // Acionado quando o mouse entra no botão
    public void OnPointerEnter(PointerEventData eventData)
    {
        tamanhoAlvo = tamanhoDesejado;
    }

    // Acionado quando o mouse sai do botão
    public void OnPointerExit(PointerEventData eventData)
    {
        tamanhoAlvo = tamanhoOriginal;
    }
}
