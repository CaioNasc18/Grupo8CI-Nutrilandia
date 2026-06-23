using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LetterCell : MonoBehaviour
{
    public TMP_Text letterText;
    public Button button;
    public Image background;

    public int x;
    public int y;

    public char Letter { get; private set; }

    public System.Action<LetterCell> onClick;

    public bool IsFound;

    void Awake()
    {
        button.onClick.AddListener(() =>
        {
            onClick?.Invoke(this);
        });
    }

    public void SetLetter(char letter)
    {
        Letter = letter;
        letterText.text = letter.ToString();
    }

    public void SelectColor()
    {
        if (!IsFound)
            background.color = new Color(0.6f, 0.3f, 0.8f);
    }

    public void HighlightCorrect()
    {
        background.color = Color.yellow;
        IsFound = true;
    }

    public void ResetColor()
    {
        if (!IsFound)
            background.color = Color.white;
    }
}