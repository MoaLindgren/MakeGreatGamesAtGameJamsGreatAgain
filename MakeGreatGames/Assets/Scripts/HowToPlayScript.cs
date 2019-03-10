using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HowToPlayScript : MonoBehaviour
{
    public void Return()
    {
        SceneManager.LoadScene(0);
    }
}
