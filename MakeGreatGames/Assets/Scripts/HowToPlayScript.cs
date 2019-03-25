using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HowToPlayScript : MonoBehaviour
{
    [SerializeField]
    GameObject[] pages;

    [SerializeField]
    GameObject previousPageButton, nextPageButton;

    int currentPage = 0;

    private void Start()
    {
        ChangePage(0);
    }

    public void Return()
    {
        SceneManager.LoadScene(0);
    }

    public void ChangePage(int next)
    {
        pages[currentPage].SetActive(false);
        currentPage += next;
        pages[currentPage].SetActive(true);
        if (currentPage < 1)
            previousPageButton.SetActive(false);
        else
            previousPageButton.SetActive(true);
        if (currentPage > pages.Length - 2)
            nextPageButton.SetActive(false);
        else
            nextPageButton.SetActive(true);
    }
}