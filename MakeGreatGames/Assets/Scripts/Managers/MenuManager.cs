﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Xml;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    GameObject newGameMenu, highScoreMenu, loadingScreen;

    [SerializeField]
    InputField playerNameInput;

    [SerializeField]
    Text[] highScoreNameTexts, highScoreTexts;

    XmlDocument highscoreXML = new XmlDocument(), howToPlayXML = new XmlDocument();

    static MenuManager instance;

    Selectable currentSelectable;

    public static MenuManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        highscoreXML.Load(Application.streamingAssetsPath + "/HighScoreXML.xml");
        Time.timeScale = 1;
    }
    
    public void ShowHighScores(bool show)
    {
        highScoreMenu.SetActive(show);
        if (show)
        {
            int index = 0;
            foreach (XmlNode node in highscoreXML.SelectNodes("//Player"))
            {
                highScoreNameTexts[index].text = node.Attributes[0].Value;
                highScoreTexts[index].text = node.Attributes[1].Value;
                index++;
            }
        }
    }

    public void ShowNewGameMenu(bool show)
    {
        if (!show)
            playerNameInput.text = "";
        newGameMenu.SetActive(show);
        if (show)
        {
            playerNameInput.Select();
            playerNameInput.ActivateInputField();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void NewGame()
    {
        string playerName = playerNameInput.text;
        if (playerName != "")
        {
            using (StreamWriter writer = File.CreateText(Application.persistentDataPath + "/PlayerName.dat"))
            {
                writer.WriteLine(playerName);
            }
            LoadScene(1);
        }
    }

    void LoadScene(int sceneIndex)
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(true);
        StartCoroutine(SceneLoader(sceneIndex));
    }

    IEnumerator SceneLoader(int sceneIndex)
    {
        AsyncOperation job = SceneManager.LoadSceneAsync(sceneIndex);
        while (!job.isDone)
        {
            if (loadingScreen != null)
            {
                Slider slider = loadingScreen.GetComponentInChildren<Slider>();
                slider.value = job.progress;
                slider.gameObject.GetComponentInChildren<Text>().text = (job.progress * 100).ToString() + "%";
            }
            yield return null;
        }
    }
}