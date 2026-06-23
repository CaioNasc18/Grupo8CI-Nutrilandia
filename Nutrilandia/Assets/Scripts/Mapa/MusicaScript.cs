using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Instância estática para o padrão Singleton (garante que só exista UM AudioManager)
    public static AudioManager instance;

    private AudioSource audioSource;
    private bool isMuted = false;

    void Awake()
    {
        // Se já existir um AudioManager na cena, destrói o novo para não duplicar a música
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        // ESSA LINHA FAZ A MÁGICA: mantém o objeto vivo entre as cenas
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    // Função que o botão da UI vai chamar
    public void ToggleMusic()
    {
        isMuted = !isMuted;
        audioSource.mute = isMuted;
    }

    // Função útil para o botão saber o estado atual (opcional)
    public bool IsMuted()
    {
        return isMuted;
    }
}