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
    GameObject specialAttackImage;
    [SerializeField]
    Sprite[] specialAttacksSprites;
    [SerializeField]
    Sprite specialAttackDefaultSprite;
    [SerializeField]
    GameObject pauseMenu, pauseMainMenu;
    [SerializeField]
    int maxNumberOfCoins;
    int coins, score;
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

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;
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
            countCoinsText.text = coins.ToString();
        }
    }

    public void SpecialAttack(bool specialAttack, int timer, int specialAttackIndex)
    {
        if(specialAttack)
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
        if(specialAttack)
        {
            counter -= Time.deltaTime;
            specialAttackTimer.gameObject.SetActive(true);
            specialAttackTimer.text = counter.ToString("0");
            if(counter <= 0)
            {
                SpecialAttack(false, 0, 0);
            }
        }
        if(GameManager.Instance.Paused)
        {
            pauseMenu.SetActive(true);
        }
        else
        {
            pauseMenu.SetActive(false);
        }
    }

    public void OpenCofirmMenu(GameObject confirm)
    {
        pauseMainMenu.SetActive(false);
        confirm.SetActive(true);
    }
    public void Back(GameObject closeThis)
    {
        pauseMainMenu.SetActive(true);
        closeThis.SetActive(false);
        if (closeThis.name == "PauseMenu")
        {
            GameManager.Instance.Paused = false;
            GameManager.Instance.PauseAndUnpause(false);
        }
    }
    public void Restart()
    {
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
