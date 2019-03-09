using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.IO;
using System.Xml;
using System.Xml.XPath;

struct PlayerInfo
{
    int score;
    string name;

    public int Score
    {
        get { return score; }
    }

    public string Name
    {
        get { return name; }
    }

    public PlayerInfo(int score, string name)
    {
        this.score = score;
        this.name = name;
    }
}

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject pauseMenu;

    XmlDocument highScoreXml = new XmlDocument();

    XPathNavigator xNav;

    int score = 0;

    static GameManager instance;

    public static GameManager Instance
    {
        get { return instance; }
    }

    bool paused = false;

    public bool Paused
    {
        get { return paused; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        instance = this;
        highScoreXml.Load(Application.streamingAssetsPath + "/HighScoreXML.xml");
        xNav = highScoreXml.CreateNavigator();
    }

    public void TankDestroyed(TankScript tank)
    {
        if (tank is PlayerScript)
        {
            GameOver();
        }
        else
        {
            score += ((NpcScript)tank).Points;
        }
    }

    public void PauseAnUnpause(bool pause)
    {
        if (pause)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
        pauseMenu.SetActive(pause);
    }

    void GameOver()
    {
        Time.timeScale = 0f;
        PlayerInfo[] highScores = new PlayerInfo[5];
        int index = 0;
        foreach(XmlNode node in xNav.Select("//Player/@Name"))
        {
            //highScores[i] = new PlayerInfo(node())
            index++;
        }
    }
}