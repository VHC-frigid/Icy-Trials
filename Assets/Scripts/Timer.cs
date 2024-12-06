using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public TMP_Text timerText;
    public float timer;

    private void Start()
    {
        Time.timeScale = 1;
    }

    private void Update()
    {
        float timerRound = timer;
        timerRound = Mathf.Round(timerRound * 100);
        
        timerText.text = timerRound.ToString();

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            SceneManager.LoadScene(0);
            Time.timeScale = 0;
        }
    }
}
