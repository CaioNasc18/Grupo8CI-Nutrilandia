using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class WordSearchGenerator : MonoBehaviour
{
    public LetterCell cellPrefab;
    public Transform gridParent;
    public int width = 10;
    public int height = 10;
    private char[,] grid;
    private List<LetterCell> selectedCells = new();
    private int wordsFound = 0;
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    public Difficulty difficulty;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        grid = new char[width, height];

        PlaceWords();
        FillEmptyCells();
        DrawGrid();
    }

    void DrawGrid()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                LetterCell cell = Instantiate(cellPrefab, gridParent);

                cell.x = x;
                cell.y = y;

                cell.SetLetter(grid[x, y]);
                cell.onClick += OnCellClicked;
            }
        }
    }

//palavra p dificuldades
    string[] GetWords()
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                return new string[] { "LEITE", "BANANA", "OVO", "AGUA", "TOMATE" };
            case Difficulty.Medium:
                return new string[] { "FRUTAS", "CENOURA", "ALFACE", "PEIXE", "CEREAIS" };
            case Difficulty.Hard:
                return new string[] { "FRUTAS", "PEIXE", "LEGUMES", "CENOURA", "ALFACE" };
        }

        return new string[0];
    }

//gerar palavras auto
    void PlaceWords()
    {
        string[] words = GetWords();
        foreach (string word in words)
        {
            bool placed = false;
            int attempts = 0;

            while (!placed && attempts < 100)
            {
                attempts++;

                int dx = Random.Range(0, 2);
                int dy = (dx == 0) ? 1 : 0;

                int startX = Random.Range(0, width);
                int startY = Random.Range(0, height);

                int endX = startX + dx * (word.Length - 1);
                int endY = startY + dy * (word.Length - 1);

                if (endX >= width || endY >= height)
                    continue;
                bool canPlace = true;

                for (int i = 0; i < word.Length; i++)
                {
                    int x = startX + dx * i;
                    int y = startY + dy * i;

                    if (grid[x, y] != '\0' && grid[x, y] != word[i])
                    {
                        canPlace = false;
                        break;
                    }
                }

                if (canPlace)
                {
                    for (int i = 0; i < word.Length; i++)
                    {
                        int x = startX + dx * i;
                        int y = startY + dy * i;
                        grid[x, y] = word[i];
                    }

                    placed = true;
                }
            }
        }
    }

    void FillEmptyCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == '\0')
                    grid[x, y] = (char)Random.Range('A', 'Z' + 1);
            }
        }
    }

//clickar

    void OnCellClicked(LetterCell cell)
    {
        if (cell.IsFound)
            return;
        if (selectedCells.Contains(cell))
            return;

        selectedCells.Add(cell);
        cell.SelectColor();
        CheckSelection();
    }

//palavras encontradas

    void CheckSelection()
    {
        string[] words = GetWords();

        foreach (string word in words)
        {
            if (IsMatch(word))
            {
                foreach (var cell in selectedCells)
                    cell.HighlightCorrect();
                wordsFound++;
                CheckWin();
                selectedCells.Clear();
                return;
            }
        }
        if (selectedCells.Count > 15)
            CancelSelection();
    }

    bool IsMatch(string word)
    {
        if (selectedCells.Count != word.Length)
            return false;

        for (int i = 0; i < word.Length; i++)
        {
            if (selectedCells[i].Letter != word[i])
                return false;
        }

        return true;
    }

//botao reset

    public void CancelSelection()
    {
        foreach (var cell in selectedCells)
            cell.ResetColor();
        selectedCells.Clear();
    }

//win scene
    void CheckWin()
    {
        if (wordsFound >= GetWords().Length)
        {
            Debug.Log("YOU WIN!");
            LoadWinScene();
        }
    }

    void LoadWinScene()
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                SceneManager.LoadScene("EasyWinScene");
                break;
            case Difficulty.Medium:
                SceneManager.LoadScene("MediumWinScene");
                break;
            case Difficulty.Hard:
                SceneManager.LoadScene("HardWinScene");
                break;
        }
    }
}