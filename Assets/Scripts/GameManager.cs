using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public AudioSource music;
    
    public bool startPlaying;
    
    public BeatScroller beatScroller;
    
    public static GameManager instance;

    public int currentScore = 0;
    public int scorePerNote = 100;
    public int scorePerGoodNote = 125;
    public int scorePerPerfectNote = 150;

    public int currentMultiplier = 1;
    public int multiplierTracker;
    public int[] multiplierThresholds;
    
    public Text scoreText;
    public Text multiText;

    public int totalNotes = 0;
    public int normalHits = 0;
    public int goodHits = 0;
    public int perfectHits = 0;
    public int missHits = 0;

    public GameObject resultsScreen;
    
    public Text percentHitsText;
    public Text normalHitsText;
    public Text goodHitsText;
    public Text perfectHitsText;
    public Text missHitsText;
    public Text rankText;
    public Text finalScoreText;

    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        scoreText.text = "Score: " + currentScore;
        currentMultiplier = 1;
        totalNotes = FindObjectsByType<NoteObject>(FindObjectsSortMode.None).Length;
    }

    // Update is called once per frame
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
        } else if (!music.isPlaying && !resultsScreen.activeSelf)
        {
            resultsScreen.SetActive(true);
            
            normalHitsText.text = "" + normalHits;
            goodHitsText.text = "" + goodHits;
            perfectHitsText.text = "" + perfectHits;
            missHitsText.text = "" + missHits;
            float percentage = ((float)(totalNotes - missHits) / totalNotes) * 100f;
            percentHitsText.text = percentage.ToString("F1") + "%";
            finalScoreText.text = "" + currentScore;

            string rankVal = "F";
            if (percentage > 40) rankVal = "D";
            if  (percentage > 55) rankVal = "C";
            if (percentage > 70) rankVal = "B";
            if (percentage > 85) rankVal = "A";
            if (percentage > 95) rankVal = "S";
            if (Mathf.Approximately(percentage, 100)) rankVal = "S+";
            
            rankText.text = rankVal;
        }
    }

    public void NoteHit()
    {
        Debug.Log("Note Hit On Time");

        if (currentMultiplier - 1 < multiplierThresholds.Length)
        {
            multiplierTracker++;

            if (multiplierThresholds[currentMultiplier - 1] <= multiplierTracker)
            {
                multiplierTracker = 0;
                currentMultiplier++;
            }
        }
        
        //currentScore += scorePerNote * currentMultiplier;
        scoreText.text = "Score: " + currentScore;
        multiText.text = "Multiplier: x" + currentMultiplier;
        
        Debug.Log("Score: " + currentScore);
        Debug.Log("Multiplier: " + currentMultiplier);
    }

    public void NormalHit()
    {
        currentScore += scorePerNote * currentMultiplier;
        NoteHit();
        normalHits++;
    }

    public void GoodHit()
    {
        currentScore += scorePerGoodNote * currentMultiplier;
        NoteHit();
        goodHits++;
    }
    
    public void PerfectHit()
    {
        currentScore += scorePerPerfectNote * currentMultiplier;
        NoteHit();
        perfectHits++;
    }

    public void NoteMissed()
    {
        Debug.Log("Note Missed");
        
        currentMultiplier = 1;
        multiplierTracker = 0;
        missHits++;

        multiText.text = "Multiplier: x" + currentMultiplier;
    }
}
