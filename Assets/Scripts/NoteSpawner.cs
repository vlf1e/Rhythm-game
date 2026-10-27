using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public SongData fallbackSong; // Песня по умолчанию, если запустили сцену сразу без меню
    public Transform[] laneSpawnPoints; // 4 точки по X для каждой дорожки
    public KeyCode[] laneKeys = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K };
    public float activatorY = -3.5f; // Координата Y, где находятся кнопки нажатия
    public float spawnY = 6f;        // Координата Y, где появляются ноты сверху

    [Header("Префабы стрелок по дорожкам (4 шт: 0-Лево, 1-Низ, 2-Верх, 3-Право)")]
    public GameObject[] normalNotePrefabs; // Массив из 4 обычных стрелок
    public GameObject[] longNotePrefabs;   // Массив из 4 длинных стрелок

    public Transform notesContainer; // Объект с BeatScroller

    private SongData _currentSong;
    private DifficultyChart _currentChart;
    private float _secPerBeat;
    private float _fallTime;
    private int _nextNoteIndex = 0;
    private List<NoteData> _sortedNotes;

    public int InitSpawner(AudioSource audioSource, BeatScroller scroller)
    {
        _currentSong = GameSettings.selectedSong != null ? GameSettings.selectedSong : fallbackSong;
        int diffIndex = Mathf.Clamp(GameSettings.selectedDifficultyIndex, 0, _currentSong.difficulties.Length - 1);
        _currentChart = _currentSong.difficulties[diffIndex];

        audioSource.clip = _currentSong.musicClip;
        _secPerBeat = 60f / _currentSong.bpm;

        // Настраиваем скорость движения контейнера (юнитов в секунду)
        scroller.fallSpeed = _currentChart.fallSpeed;

        // Время, за которое нота долетает от spawnY до activatorY
        float distance = spawnY - activatorY;
        _fallTime = distance / _currentChart.fallSpeed;

        _sortedNotes = new List<NoteData>(_currentChart.notes);

        // Если карта нот пустая, автоматически сгенерируем ноты по битам для лабораторной
        if (_sortedNotes.Count == 0 && _currentSong.musicClip != null)
        {
            GenerateAutoBeatmap(_currentSong.musicClip.length, diffIndex);
        }

        _sortedNotes.Sort((a, b) => a.beatToHit.CompareTo(b.beatToHit));
        _nextNoteIndex = 0;

        return _sortedNotes.Count;
    }

    void Update()
    {
        if (!GameManager.instance.startPlaying || _sortedNotes == null) return;

        float songTime = GameManager.instance.music.time;

        while (_nextNoteIndex < _sortedNotes.Count)
        {
            NoteData noteData = _sortedNotes[_nextNoteIndex];
            float hitTimeSec = noteData.beatToHit * _secPerBeat;

            // Спавним ноту ровно за _fallTime секунд до момента её удара
            if (songTime >= hitTimeSec - _fallTime)
            {
                SpawnNote(noteData, hitTimeSec, songTime);
                _nextNoteIndex++;
            }
            else
            {
                break;
            }
        }
    }

    private void SpawnNote(NoteData data, float hitTimeSec, float currentSongTime)
    {
        int lane = Mathf.Clamp(data.laneIndex, 0, 3);
    
        // Вычисляем точную позицию по Y для идеальной синхронизации
        float timeUntilHit = hitTimeSec - currentSongTime;
        float exactY = activatorY + (timeUntilHit * _currentChart.fallSpeed);

        Vector3 spawnPos = new Vector3(laneSpawnPoints[lane].position.x, exactY, 0f);

        // Выбираем префаб конкретной стрелки под нужную дорожку (lane)
        GameObject prefab = data.isLongNote ? longNotePrefabs[lane] : normalNotePrefabs[lane];

        GameObject noteObj = Instantiate(prefab, spawnPos, Quaternion.identity, notesContainer);
        NoteObject noteScript = noteObj.GetComponent<NoteObject>();

        float holdDurationSec = data.isLongNote ? data.beatDuration * _secPerBeat : 0f;
        noteScript.Setup(laneKeys[lane], activatorY, data.isLongNote, holdDurationSec, _currentChart.fallSpeed);
    }

    // Автогенератор нот в такт BPM (выручит, если не хочется прописывать сотни нот руками)
    private void GenerateAutoBeatmap(float songLengthSec, int difficulty)
    {
        int totalBeats = Mathf.FloorToInt(songLengthSec / _secPerBeat);
        float step = difficulty == 0 ? 2f : (difficulty == 1 ? 1f : 0.5f); // Частота нот зависит от сложности

        Random.InitState(_currentSong.songTitle.GetHashCode() + difficulty);

        for (float beat = 4f; beat < totalBeats - 4f; beat += step)
        {
            bool makeLong = Random.value < 0.2f; // 20% шанс на длинную ноту
            float duration = makeLong ? step * 1.5f : 0f;

            _sortedNotes.Add(new NoteData
            {
                laneIndex = Random.Range(0, 4),
                beatToHit = beat,
                isLongNote = makeLong,
                beatDuration = duration
            });

            if (makeLong) beat += duration; // Пропускаем биты, пока тянется длинная нота
        }
    }
}