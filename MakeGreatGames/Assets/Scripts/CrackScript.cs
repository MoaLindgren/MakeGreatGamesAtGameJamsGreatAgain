using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackScript : MonoBehaviour, IPoolable
{
    [SerializeField]
    float renderTime;

    bool active = false;

    Renderer r;

    void Start()
    {
        if (r == null)
        {
            r = GetComponent<Renderer>();
        }
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(renderTime);
        GameManager.Instance.CrackPool.RePoolObject(gameObject);
    }

    public void DeActivate()
    {

    }

    public void Activate()
    {
        StartCoroutine("Destroy");
    }

    public bool IsActive()
    {
        return active;
    }
}