using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Xml;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    GameObject newGameMenu, highScoreMenu;

    [SerializeField]
    InputField playerNameInput;

    [SerializeField]
    Text[] highScoreNameTexts, highScoreTexts;

    XmlDocument highscoreDoc = new XmlDocument();

    static MenuManager instance;

    public static MenuManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        highscoreDoc.Load(Application.streamingAssetsPath + "/HighScoreXML.xml");
        Time.timeScale = 1;
    }

    public void ShowHighScores(bool show)
    {
        highScoreMenu.SetActive(show);
        if (show)
        {
            int index = 0;
            foreach (XmlNode node in highscoreDoc.SelectNodes("//Player"))
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
        SceneManager.LoadScene(sceneIndex);
    }
}