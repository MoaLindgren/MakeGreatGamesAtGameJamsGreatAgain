using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    Text countCoinsText, specialAttackTimer, scoreText;

    [SerializeField]
    Slider coinProgressSlider;

    [SerializeField]
    GameObject pauseMenu, pauseMainMenu, gameOverMenu, readyText;

    [SerializeField]
    Sprite[] specialAttacksSprites;

    [SerializeField]
    Sprite specialAttackDefaultSprite;

    [SerializeField]
    Image specialAttackImage;

    int coins, score, maxNumberOfCoins;

    float counter;

    bool specialAttack;

    static UIManager instance;

    public static UIManager Instance
    {
        get { return instance; }
    }

    public int Coins
    {
        set
        {
            coins = value;
            coinProgressSlider.value = coins * coinProgressSlider.maxValue / maxNumberOfCoins;
            if (coins == 0)
            {
                coinProgressSlider.value = coinProgressSlider.minValue;
            }
            if (coins >= 3)
            {
                CoinsCollected(true);
            }
            else
            {
                CoinsCollected(false);
            }
            countCoinsText.text = coins.ToString();
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        instance = this;
    }

    private void Start()
    {
        maxNumberOfCoins = CoinManager.Instance.CoinsToUlt;
        coinProgressSlider.maxValue = maxNumberOfCoins;
    }

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;
    }

    public void SpinWheel()
    {
        AudioSource spinSound = AudioManager.Instance.SpawnSound("SpinSound", transform, true, false, true, 0.65f);
    }

    public void SpecialAttack(bool specialAttack, int timer, int specialAttackIndex)
    {
        this.specialAttack = specialAttack;
        if (specialAttack)
        {
            counter = timer;
            specialAttackTimer.gameObject.SetActive(true);
            specialAttackImage.sprite = specialAttacksSprites[specialAttackIndex];
        }
        else
        {
            specialAttackTimer.gameObject.SetActive(false);
            specialAttackImage.sprite = specialAttackDefaultSprite;
        }
    }

    void Update()
    {
        if (specialAttack)
        {
            counter -= Time.deltaTime;
            specialAttackTimer.text = counter.ToString("0");
            if (counter <= 0)
            {
                SpecialAttack(false, 0, 0);
            }
        }
    }

    public void OpenCofirmMenu(GameObject confirm)
    {
        pauseMainMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        confirm.SetActive(true);
    }

    public void Back(GameObject closeThis)
    {
        pauseMainMenu.SetActive(true);
        gameOverMenu.SetActive(true);
        closeThis.SetActive(false);
        if (closeThis.name == "PauseMenu")
        {
            GameManager.Instance.Paused = false;
            GameManager.Instance.PauseAndUnpause(false);
        }
    }

    public void CoinsCollected(bool fullyCollected)
    {
        readyText.SetActive(fullyCollected);
    }

    public void Pause(bool pause)
    {
        pauseMenu.SetActive(pause);
    }

    public void Restart()
    {
        GameManager.Instance.RestartGame();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}