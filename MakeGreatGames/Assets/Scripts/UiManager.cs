using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField]
    Text countCoinsText, specialAttackTimer;
    [SerializeField]
    Slider coinProgressSlider;
    [SerializeField]
    GameObject specialAttackActiveImage;
    [SerializeField]
    int maxNumberOfCoins;
    int coins;
    float counter;
    bool specialAttack;

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
    public void SpecialAttack(bool specialAttack, int timer)
    {
        this.specialAttack = specialAttack;
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
                SpecialAttack(false, 0);
            }
        }
    }
}
