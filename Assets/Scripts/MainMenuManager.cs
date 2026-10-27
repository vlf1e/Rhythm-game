using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Список всех песен (минимум 3)")]
    public SongData[] availableSongs;

    [Header("Панели меню")]
    public GameObject mainPanel;
    public GameObject songSelectPanel;
    public GameObject leaderboardPanel;

    [Header("UI выбора песни и сложности")]
    public Dropdown songDropdown;
    public Dropdown difficultyDropdown;

    [Header("UI Таблицы рекордов")]
    public Text leaderboardText;

    public string gameSceneName = "GameScene";

    void Start()
    {
        PopulateDropdowns();
        ShowMainPanel();
    }

    private void PopulateDropdowns()
    {
        if (availableSongs.Length == 0) return;

        if (songDropdown != null)
        {
            songDropdown.ClearOptions();
            List<string> songNames = new List<string>();
            foreach (var song in availableSongs)
            {
                songNames.Add(song.songTitle);
            }
            songDropdown.AddOptions(songNames);
            songDropdown.onValueChanged.AddListener(OnSongChanged);
        }

        if (difficultyDropdown != null)
        {
            difficultyDropdown.ClearOptions();
            difficultyDropdown.AddOptions(new List<string> { "Легкий (Easy)", "Средний (Normal)", "Сложный (Hard)" });
            difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);
        }

        // Значения по умолчанию
        GameSettings.selectedSong = availableSongs[0];
        GameSettings.selectedDifficultyIndex = 0;
    }

    public void OnSongChanged(int index)
    {
        if (index >= 0 && index < availableSongs.Length)
        {
            GameSettings.selectedSong = availableSongs[index];
        }
    }

    public void OnDifficultyChanged(int index)
    {
        GameSettings.selectedDifficultyIndex = index;
    }

    // Кнопка "Начало игры"
    public void StartGame()
    {
        if (GameSettings.selectedSong == null && availableSongs.Length > 0)
        {
            GameSettings.selectedSong = availableSongs[0];
        }
        SceneManager.LoadScene(gameSceneName);
    }

    // Кнопка "Выбор песни и сложности"
    public void OpenSongSelection()
    {
        mainPanel.SetActive(false);
        leaderboardPanel.SetActive(false);
        songSelectPanel.SetActive(true);
    }

    // Кнопка "Таблица рекордов"
    public void OpenLeaderboard()
    {
        mainPanel.SetActive(false);
        songSelectPanel.SetActive(false);
        leaderboardPanel.SetActive(true);
        RefreshLeaderboardUI();
    }

    // Кнопка "Назад" на подпанелях
    public void ShowMainPanel()
    {
        mainPanel.SetActive(true);
        if (songSelectPanel != null) songSelectPanel.SetActive(false);
        if (leaderboardPanel != null) leaderboardPanel.SetActive(false);
    }

    // Кнопка "Выход"
    public void QuitGame()
    {
        Debug.Log("Выход из игры");
        Application.Quit();
    }

    private void RefreshLeaderboardUI()
    {
        if (leaderboardText == null) return;

        HighScoreList list = GameSettings.LoadScores();
        if (list.entries.Count == 0)
        {
            leaderboardText.text = "Рекордов пока нет. Сыграйте первую партию!";
            return;
        }

        StringBuilder sb = new StringBuilder();
        int count = Mathf.Min(list.entries.Count, 10); // Топ-10

        for (int i = 0; i < count; i++)
        {
            var e = list.entries[i];
            sb.AppendLine($"{i + 1}. {e.playerName} | {e.songTitle} ({e.difficulty}) — Счёт: {e.score} | Комбо: {e.maxCombo} | Точность: {e.accuracy:F1}%");
        }

        leaderboardText.text = sb.ToString();
    }
}