using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private Hole[] holes;

    [SerializeField] private Transform water;
    private MeshRenderer waterMeshRenderer;
    private float originalWaterHeight;
    public float currentWaterHeight { get; private set; }

    [SerializeField] private float minGameLengthInSeconds = 60f;
    private float timer = 0;
    private float timeAlive = 0;

    public bool hasGameStarted { get; private set; } = false;
    public bool isGameOver { get; private set; } = false;

    public delegate void OnWaterHeightDrop(float waterHeightRatio);
    public OnWaterHeightDrop onWaterHeightDrop;

    public delegate void OnScore(int score);
    public OnScore onScore;

    public delegate void OnGameEnd();
    public OnGameEnd onGameEnd;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        holes = FindObjectsOfType<Hole>();
        waterMeshRenderer = water.GetComponent<MeshRenderer>();
        originalWaterHeight = water.position.y;

        currentWaterHeight = originalWaterHeight;

        water.position = new Vector3(water.position.x, Mathf.Clamp(originalWaterHeight, 0.25f, originalWaterHeight), water.position.z);
        waterMeshRenderer.material.SetColor("_BaseColor", new Color(1, 1, 1, Mathf.Clamp(1, 0.25f, 0.8f)));
        onWaterHeightDrop?.Invoke(1);
    }

    private void Update()
    {
        if (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") > 0)
        {
            hasGameStarted = true;
        }

        if (isGameOver || !hasGameStarted)
        {
            return;
        }

        var holeRatio = GetHoleRatio();

        timer += Time.deltaTime * holeRatio;
        timeAlive += Time.deltaTime;
        CalculateScore();

        var remainingTimeRatio = timer / minGameLengthInSeconds;

        currentWaterHeight = Mathf.Clamp(originalWaterHeight - (originalWaterHeight * remainingTimeRatio), 0.15f, originalWaterHeight);
        water.position = new Vector3(water.position.x, currentWaterHeight, water.position.z);
        waterMeshRenderer.material.SetColor("_BaseColor", new Color(1, 1, 1, Mathf.Clamp(1 - remainingTimeRatio, 0.25f, 0.8f)));
        onWaterHeightDrop?.Invoke( 1 - remainingTimeRatio);

        if (timer > minGameLengthInSeconds && !isGameOver)
        {
            EndGame();
        }
    }

    private float GetHoleRatio()
    {
        float pluggedHoleCount = 0;
        foreach (var hole in holes)
        {
            if (hole.isPlugged)
            {
                pluggedHoleCount++;
            }
        }

        return 1 - (pluggedHoleCount / (pluggedHoleCount + 1));
    }

    private void CalculateScore()
    {
        var score = Mathf.CeilToInt(timeAlive);
        if (PlayerPrefs.GetInt("HighScore") < score)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
        onScore?.Invoke(score);
    }

    private void EndGame()
    {
        isGameOver = true;
        onGameEnd?.Invoke();
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}