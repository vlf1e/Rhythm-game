using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public AudioSource music;
    public BeatScroller beatScroller;
    public NoteSpawner noteSpawner;
    public bool startPlaying;

    [Header("Очки и Комбо")]
    public int currentScore = 0;
    public int scorePerNote = 100;
    public int scorePerGoodNote = 125;
    public int scorePerPerfectNote = 150;

    public int currentCombo = 0;
    public int maxCombo = 0;

    public int currentMultiplier = 1;
    public int multiplierTracker;
    public int[] multiplierThresholds;

    [Header("Игровой UI")]
    public Text scoreText;
    public Text comboText;
    public Text multiText;

    [Header("Статистика попаданий")]
    public int totalNotes = 0;
    public int normalHits = 0;
    public int goodHits = 0;
    public int perfectHits = 0;
    public int missHits = 0;

    [Header("Экран результатов")]
    public GameObject resultsScreen;
    public Text percentHitsText;
    public Text normalHitsText;
    public Text goodHitsText;
    public Text perfectHitsText;
    public Text missHitsText;
    public Text rankText;
    public Text finalScoreText;
    public Text maxComboText;
    public InputField playerNameInput;
    public Button saveScoreButton;

    private float _finalAccuracy = 0f;
    private bool _scoreSaved = false;

    void Start()
    {
        instance = this;
        resultsScreen.SetActive(false);

        scoreText.text = "Score: 0";
        if (comboText != null) comboText.text = "Combo: 0";
        multiText.text = "Multiplier: x1";

        currentMultiplier = 1;

        if (noteSpawner != null)
        {
            totalNotes = noteSpawner.InitSpawner(music, beatScroller);
        }
        else
        {
            totalNotes = FindObjectsByType<NoteObject>(FindObjectsSortMode.None).Length;
        }
    }

    void Update()
    {
        if (!startPlaying)
        {
            if (Input.anyKeyDown)
            {
                startPlaying = true;
                beatScroller.hasStarted = true;
                music.Play();
            }
        }
        else if (!music.isPlaying && !resultsScreen.activeSelf)
        {
            ShowResults();
        }
    }

    private void ShowResults()
    {
        resultsScreen.SetActive(true);

        normalHitsText.text = normalHits.ToString();
        goodHitsText.text = goodHits.ToString();
        perfectHitsText.text = perfectHits.ToString();
        missHitsText.text = missHits.ToString();

        int totalJudged = normalHits + goodHits + perfectHits + missHits;
        int hitCount = normalHits + goodHits + perfectHits;
        _finalAccuracy = totalJudged > 0 ? ((float)hitCount / totalJudged) * 100f : 0f;

        percentHitsText.text = _finalAccuracy.ToString("F1") + "%";
        finalScoreText.text = currentScore.ToString();
        if (maxComboText != null) maxComboText.text = maxCombo.ToString();

        string rankVal = "F";
        if (_finalAccuracy > 40) rankVal = "D";
        if (_finalAccuracy > 55) rankVal = "C";
        if (_finalAccuracy > 70) rankVal = "B";
        if (_finalAccuracy > 85) rankVal = "A";
        if (_finalAccuracy > 95) rankVal = "S";
        if (Mathf.Approximately(_finalAccuracy, 100f)) rankVal = "S+";

        rankText.text = rankVal;
    }

    public void NoteHit()
    {
        currentCombo++;
        if (currentCombo > maxCombo)
        {
            maxCombo = currentCombo;
        }

        if (currentMultiplier - 1 < multiplierThresholds.Length)
        {
            multiplierTracker++;
            if (multiplierThresholds[currentMultiplier - 1] <= multiplierTracker)
            {
                multiplierTracker = 0;
                currentMultiplier++;
            }
        }

        UpdateUI();
    }

    public void AddHoldPoints(int points)
    {
        currentScore += points * currentMultiplier;
        UpdateUI();
    }

    public void NormalHit()
    {
        currentScore += scorePerNote * currentMultiplier;
        normalHits++;
        NoteHit();
    }

    public void GoodHit()
    {
        currentScore += scorePerGoodNote * currentMultiplier;
        goodHits++;
        NoteHit();
    }

    public void PerfectHit()
    {
        currentScore += scorePerPerfectNote * currentMultiplier;
        perfectHits++;
        NoteHit();
    }

    public void NoteMissed()
    {
        currentCombo = 0;
        currentMultiplier = 1;
        multiplierTracker = 0;
        missHits++;

        UpdateUI();
    }

    private void UpdateUI()
    {
        scoreText.text = "Score: " + currentScore;
        if (comboText != null) comboText.text = "Combo: " + currentCombo;
        multiText.text = "Multiplier: x" + currentMultiplier;
    }

    // Привяжите этот метод к кнопке "Сохранить результат" на экране результатов
    public void SaveResultAndReturnToMenu()
    {
        if (_scoreSaved) return;
        _scoreSaved = true;

        string playerName = playerNameInput != null ? playerNameInput.text : "Player";
        GameSettings.SaveScore(playerName, currentScore, maxCombo, _finalAccuracy);

        if (saveScoreButton != null) saveScoreButton.interactable = false;
        SceneManager.LoadScene("MainMenu"); // Убедитесь, что сцена меню называется MainMenu
    }
}