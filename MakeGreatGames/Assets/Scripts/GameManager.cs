using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

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

    int score = 0;

    Camera cam;

    static GameManager instance;

    string playerName = "Anders Ramses";

    bool paused = false;

    public bool Paused
    {
        get { return paused; }
    }

    public static GameManager Instance
    {
        get { return instance; }
    }

    public Camera Cam
    {
        get { return cam; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        instance = this;
        Cursor.visible = false;
        highScoreXml.Load(Application.streamingAssetsPath + "/HighScoreXML.xml");
        cam = FindObjectOfType<Camera>();
        if (File.Exists(Application.persistentDataPath + "/PlayerName.dat"))
        {
            using(StreamReader reader = File.OpenText(Application.persistentDataPath + "/PlayerName.dat"))
            {
                playerName = reader.ReadLine();
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseAndUnpause(!paused);
        }
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
            WaveSpawner.Instance.TankDestroyed(tank.gameObject);
        }
    }

    public void PauseAndUnpause(bool pause)
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
        foreach (XmlNode node in highScoreXml.SelectNodes("//Player"))
        {
            highScores[index] = new PlayerInfo(int.Parse(node.Attributes[1].Value), node.Attributes[0].Value);
            index++;
        }
        for (int i = highScores.Length - 1; i > -1; i--)
        {
            if (score > highScores[i].Score)
            {
                for (int j = highScores.Length; j > i - 1; j--)
                {
                    highScores[j] = j == i + 1 ? new PlayerInfo(score, playerName) : highScores[j - 1];
                }
                break;
            }
        }
        highScoreXml.Save(Application.streamingAssetsPath + "/HighScoreXML.xml");
    }
}