using UnityEngine;
using UnityEngine.UI;

public class MuteButtonHandler : MonoBehaviour
{
    void Start()
    {
        // Encontra o botão neste objeto e adiciona o clique via código
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnButtonClicked);
        }
    }

    void OnButtonClicked()
    {
        // Procura o AudioManager que sobreviveu e manda ele mutar/desmutar
        if (AudioManager.instance != null)
        {
            AudioManager.instance.ToggleMusic();
        }
    }
}