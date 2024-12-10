using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinFinalLevel : MonoBehaviour
{
    public GameObject player;
    public GameObject water;
    public GameObject winScreen;

    public void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player");
    }

    public void OnTriggerEnter(Collider other)
    {
        Time.timeScale = 0;
        winScreen.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //player.GetComponent<RBCharacterController>().enabled = false;
        //water.GetComponent<RisingWater>().enabled = false;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
