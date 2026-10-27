using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HighScoreEntry
{
    public string playerName;
    public string songTitle;
    public string difficulty;
    public int score;
    public int maxCombo;
    public float accuracy;
}

[System.Serializable]
public class HighScoreList
{
    public List<HighScoreEntry> entries = new List<HighScoreEntry>();
}

public static class GameSettings
{
    public static SongData selectedSong;
    public static int selectedDifficultyIndex = 0;

    private const string LeaderboardKey = "RhythmGame_Leaderboard";

    public static void SaveScore(string playerName, int score, int maxCombo, float accuracy)
    {
        HighScoreList list = LoadScores();
        
        HighScoreEntry newEntry = new HighScoreEntry
        {
            playerName = string.IsNullOrWhiteSpace(playerName) ? "Игрок" : playerName,
            songTitle = selectedSong != null ? selectedSong.songTitle : "Test Song",
            difficulty = selectedSong != null ? selectedSong.difficulties[selectedDifficultyIndex].difficultyName : "Normal",
            score = score,
            maxCombo = maxCombo,
            accuracy = accuracy
        };

        list.entries.Add(newEntry);
        // Сортируем по убыванию очков
        list.entries.Sort((a, b) => b.score.CompareTo(a.score));

        string json = JsonUtility.ToJson(list);
        PlayerPrefs.SetString(LeaderboardKey, json);
        PlayerPrefs.Save();
    }

    public static HighScoreList LoadScores()
    {
        if (!PlayerPrefs.HasKey(LeaderboardKey))
            return new HighScoreList();

        string json = PlayerPrefs.GetString(LeaderboardKey);
        return JsonUtility.FromJson<HighScoreList>(json) ?? new HighScoreList();
    }
}