using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    [SerializeField]
    Text countCoinsText, specialAttackTimer, scoreText;

    [SerializeField]
    Slider coinProgressSlider;

    [SerializeField]
    GameObject specialAttackImage, pauseMenu, pauseMainMenu, gameOverMenu, readyText;

    [SerializeField]
    Sprite[] specialAttacksSprites;

    [SerializeField]
    Sprite specialAttackDefaultSprite;

    [SerializeField]
    AudioSource spinningSound;

    int coins, score, maxNumberOfCoins;

    float counter;

    bool specialAttack;

    static UiManager instance;

    public static UiManager Instance
    {
        get { return instance; }
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
        spinningSound.Play();
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

    public void SpecialAttack(bool specialAttack, int timer, int specialAttackIndex)
    {
        if (specialAttack)
        {
            counter = timer;
            specialAttackImage.GetComponent<Image>().sprite = specialAttacksSprites[specialAttackIndex];
        }
        else
        {
            specialAttackTimer.gameObject.SetActive(false);
            specialAttackImage.GetComponent<Image>().sprite = specialAttackDefaultSprite;
        }


    }

    void Update()
    {
        if (specialAttack)
        {
            counter -= Time.deltaTime;
            specialAttackTimer.gameObject.SetActive(true);
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

    public void Restart()
    {
        GameManager.Instance.RestartGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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