using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    public GameObject musicManager;

    private bool isPaused;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(Pause);
    }

    private void Pause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        musicManager.SetActive(!isPaused);
    }
}