using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField]
    float offsetY, offsetZ;

    GameObject player;

    private void Start()
    {
        player = FindObjectOfType<PlayerScript>().gameObject;
    }

    void LateUpdate()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + offsetY, player.transform.position.z + offsetZ);
    }
}