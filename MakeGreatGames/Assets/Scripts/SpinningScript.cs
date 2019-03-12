using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningScript : MonoBehaviour
{
    [SerializeField]
    float amount;

    void Update()
    {
        if (GameManager.Instance.Paused)
        {
            return;
        }
        transform.Rotate(0f, amount, 0f);
    }
}