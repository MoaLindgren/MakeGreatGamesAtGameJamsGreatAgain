using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(AudioSource))]
public class CoinScript : MonoBehaviour
{
    [SerializeField]
    float spinAmount;

    private void Awake()
    {
        //spawnpartiklar o skit
    }

    void Update()
    {
        transform.Rotate(0f, spinAmount, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        TankScript tank = other.GetComponent<TankScript>();
        if (tank != null)
        {
            tank.AddCoin();
            CoinManager.Instance.CoinPickedUp(gameObject);
        }
    }
}