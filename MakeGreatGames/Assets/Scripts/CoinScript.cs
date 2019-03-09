using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
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
        transform.rotation = new Quaternion(0f, transform.rotation.y + spinAmount, 0f, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        TankScript tank = other.GetComponent<TankScript>();
        if (tank != null)
        {
            tank.AddCoin();
            //sound and particles
            CoinManager.Instance.CoinPickedUp(gameObject);
        }
    }
}