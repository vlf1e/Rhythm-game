using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoteData
{
    [Tooltip("Индекс дорожки: 0, 1, 2 или 3")]
    public int laneIndex;
    [Tooltip("Удар (бит), на котором нота должна пересечь линию нажатия")]
    public float beatToHit;
    [Tooltip("Является ли нота длинной (удерживаемой)")]
    public bool isLongNote;
    [Tooltip("Длительность удержания в битах (если isLongNote = true)")]
    public float beatDuration;
}

[System.Serializable]
public class DifficultyChart
{
    public string difficultyName; // Например: Easy, Normal, Hard
    public float fallSpeed = 5f;  // Скорость падения (юнитов в секунду)
    public List<NoteData> notes = new List<NoteData>();
}

[CreateAssetMenu(fileName = "NewSong", menuName = "Rhythm Game/Song Data")]
public class SongData : ScriptableObject
{
    public string songTitle;
    public AudioClip musicClip;
    public float bpm = 120f;
    [Tooltip("Три уровня сложности: 0 - Легкий, 1 - Средний, 2 - Сложный")]
    public DifficultyChart[] difficulties = new DifficultyChart[3];
}