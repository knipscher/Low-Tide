using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject title;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [SerializeField] private string[] tutorialPrompts;

    private Hole[] holes;

    [SerializeField] private float textTime = 5f;

    private bool isInCoroutine;
    private int currentStep;

    [SerializeField] private GameObject endScreen;
    [SerializeField] private TextMeshProUGUI endText;
    [SerializeField] private Slider waterHeightBar;

    private bool hasCompletedTutorial = false;

    private void Start()
    {
        holes = FindObjectsOfType<Hole>();
        UpdateScore(0);

        endScreen.SetActive(false);
        waterHeightBar.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);

        GameManager.instance.onScore += UpdateScore;
        GameManager.instance.onGameEnd += EndGame;
        GameManager.instance.onWaterHeightDrop += SetWaterHeightBar;
    }

    private void Update()
    {
        if (!GameManager.instance.hasGameStarted)
        {
            return;
        }

        title.SetActive(false);
        scoreText.gameObject.SetActive(true);
        waterHeightBar.gameObject.SetActive(true);

        // if (PlayerPrefs.GetInt("HasCompletedTutorial") == 0)
        if (!hasCompletedTutorial)
        {
            if (tutorialText.text.Equals(""))
            {
                tutorialText.text = tutorialPrompts[0];
            }
            ProgressThroughTutorial();
        }
    }

    private void ProgressThroughTutorial()
    {   // This is certainly not the best way to make a tutorial, but it is a pretty fast way!
        switch (currentStep)
        {
            case 0:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    currentStep = 1;
                    tutorialText.text = tutorialPrompts[1];
                }
                break;

            case 1:
                foreach (var hole in holes)
                {
                    if (hole.isPlugged)
                    {
                        currentStep = 2;
                        tutorialText.text = tutorialPrompts[2];
                    }
                }
                break;

            case 2:
                if (!isInCoroutine)
                {
                    StartCoroutine(DelayThenProgress());
                }
                break;
            case 3:
                tutorialText.gameObject.SetActive(false);
                hasCompletedTutorial = true;
                // PlayerPrefs.SetInt("HasCompletedTutorial", 1);
                break;
        }
    }

    private IEnumerator DelayThenProgress()
    {
        isInCoroutine = true;
        yield return new WaitForSeconds(textTime);
        currentStep++;
        isInCoroutine = false;
    }

    private void UpdateScore(int score)
    {
        var highScore = PlayerPrefs.GetInt("HighScore");
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }

        scoreText.text = "time alive: " + score;
        highScoreText.text = "high score: " + highScore;
        endText.text = "you survived " + score + " seconds!";
    }

    private void SetWaterHeightBar(float ratio)
    {
        waterHeightBar.SetValueWithoutNotify(ratio);
    }

    private void EndGame()
    {
        tutorialText.gameObject.SetActive(false);
        endScreen.SetActive(true);
    }
}