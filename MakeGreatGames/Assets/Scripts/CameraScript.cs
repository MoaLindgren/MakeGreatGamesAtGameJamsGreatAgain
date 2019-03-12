using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField]
    bool shake;

    GameObject player;

    private void Start()
    {
        player = FindObjectOfType<PlayerScript>().gameObject;
    }

    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 10, player.transform.position.z - 5);
    }
}
