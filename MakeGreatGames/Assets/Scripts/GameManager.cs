using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject pauseMenu;

    static GameManager instance;

    public static GameManager Instance
    {
        get { return instance; }
    }

    int score = 0;

    bool paused = false;

    public bool Paused
    {
        get { return paused; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        instance = this;
    }

    public void TankDestroyed(TankScript tank)
    {
        //if tank == spelare: game over, else points++ eller nåt
    }

    public void PauseAnUnpause(bool pause)
    {
        if (pause)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
        pauseMenu.SetActive(pause);
    }

    void GameOver()
    {
        Time.timeScale = 0f;
    }
}