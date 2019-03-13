using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionScript : MonoBehaviour
{
    [SerializeField]
    float lifeTime;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("AnnihilationTimer");
    }

    IEnumerator AnnihilationTimer()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}