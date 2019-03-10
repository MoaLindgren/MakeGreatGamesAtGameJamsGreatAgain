using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField]
    Text countCoinsText, specialAttackTimer, scoreText;
    [SerializeField]
    Slider coinProgressSlider;
    [SerializeField]
    GameObject specialAttackActiveImage;
    [SerializeField]
    Sprite[] specialAttacksSprites;
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
        this.specialAttack = specialAttack;
        specialAttackActiveImage.GetComponent<Image>().sprite = specialAttacksSprites[specialAttackIndex];
        specialAttackActiveImage.SetActive(specialAttack);
        counter = timer;
    }
    void Update()
    {
        if(specialAttack)
        {
            counter -= Time.deltaTime;
            specialAttackTimer.text = counter.ToString("0");
            if(counter <= 0)
            {
                SpecialAttack(false, 0, 0);
            }
        }
    }
}
