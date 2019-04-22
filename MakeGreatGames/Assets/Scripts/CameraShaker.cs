using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    [SerializeField]
    float shakeAmount;//The amount to shake this frame.

    [SerializeField]
    float shakeDuration;//The duration this frame.

    [SerializeField]
    bool smooth;//Smooth rotation?

    [SerializeField]
    float smoothAmount = 5f;//Amount to smooth

    float shakePercentage;//A percentage (0-1) representing the amount of shake to be applied when setting rotation.
    float startAmount;//The initial shake amount (to determine percentage), set when ShakeCamera is called.
    float startDuration;//The initial shake duration, set when ShakeCamera is called.

    bool gameRunning = true; //Is the coroutine running right now?

    Coroutine currentShake;

    static CameraShaker instance;

    public static CameraShaker Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        instance = this;
    }

    public void ShakeCamera(float amount, float duration)
    {
        if (!gameRunning)
            return;
        shakeAmount += amount;//Add to the current amount.
        startAmount = shakeAmount;//Reset the start amount, to determine percentage.
        shakeDuration += duration;//Add to the current time.
        startDuration = shakeDuration;//Reset the start time.

        ControllerShaker.Instance.Shake(amount * 2, duration * 1.5f);
        if (currentShake != null)
            StopCoroutine(currentShake);
        currentShake = StartCoroutine(Shake());//Only call the coroutine if it isn't currently running. Otherwise, just set the variables.
    }

    IEnumerator Shake()
    {
        while (shakeDuration > 0.01f && GameManager.Instance.GameRunning)
        {
            if (GameManager.Instance.Paused)
                yield return null;
            Vector3 rotationAmount = Random.insideUnitSphere * shakeAmount;//A Vector3 to add to the Local Rotation
            rotationAmount.z = 0;//Don't change the Z; it looks funny.

            shakePercentage = shakeDuration / startDuration;//Used to set the amount of shake (% * startAmount).

            shakeAmount = startAmount * shakePercentage;//Set the amount of shake (% * startAmount).
            shakeDuration = Mathf.Lerp(shakeDuration, 0, Time.deltaTime);//Lerp the time, so it is less and tapers off towards the end.

            if (smooth)
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(rotationAmount), Time.deltaTime * smoothAmount);
            else
                transform.localRotation = Quaternion.Euler(rotationAmount);//Set the local rotation the be the rotation amount.

            yield return null;
        }
        transform.localRotation = Quaternion.identity;//Set the local rotation to 0 when done, just to get rid of any fudging stuff.
    }

    public void StopShaking()
    {
        ControllerShaker.Instance.Shake(0f, Time.deltaTime);
        gameRunning = false;
        shakeDuration = 0f;
        shakeAmount = 0f;
        StopCoroutine("Shake");
        transform.localRotation = Quaternion.identity;//Set the local rotation to 0 when done, just to get rid of any fudging stuff.
    }
}