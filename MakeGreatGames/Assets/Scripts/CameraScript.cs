using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField]
    GameObject player/*, floor*/;
    [SerializeField]
    bool shake;
    //[SerializeField]
    //float zOffset, xOffset;

    //float xDistance, zDistance;

    //private void Awake()
    //{

    //    xDistance = floor.GetComponent<Renderer>().bounds.size.x / 2;
    //    zDistance = floor.GetComponent<Renderer>().bounds.size.z / 2;
    //}

    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 10, player.transform.position.z - 5);

        /*new Vector3(Mathf.Clamp(player.transform.position.x, -xDistance + xOffset, xDistance - xOffset), player.transform.position.y + 10, Mathf.Clamp(player.transform.position.z - 5, -zDistance + zOffset, zDistance - zOffset))*/
    }
}
