using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

[ExecuteInEditMode]
public class ControllerShaker : MonoBehaviour
{
    static ControllerShaker instance;

    Coroutine currentShake;

    public static ControllerShaker Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        instance = this;
    }

    public void Shake(float amount, float duration)
    {
        print(duration);
        if (currentShake != null)
            StopCoroutine(currentShake);
        currentShake = StartCoroutine(ShakeTime(amount, duration));
    }

    IEnumerator ShakeTime(float amount, float duration)
    {
        while (duration > 0f)
        {
            GamePad.SetVibration(0, amount, amount);
            amount -= duration * Time.deltaTime;
            duration -= Time.deltaTime;
            yield return null;
        }
        GamePad.SetVibration(0, 0f, 0f);
    }

    private void OnDisable()
    {
        GamePad.SetVibration(0, 0f, 0f);
    }
}