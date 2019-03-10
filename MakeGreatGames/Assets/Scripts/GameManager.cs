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
    GameObject pauseMenu, gameOverScreen;

    XmlDocument highScoreXml = new XmlDocument();

    int score = 0;

    public int Score
    {
        set { score += value; }
    }

    Camera cam;

    static GameManager instance;

    string playerName = "TestPerson";

    bool paused = false;

    public bool Paused
    {
        get { return paused; }
        set { paused = value; }
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
            using (StreamReader reader = File.OpenText(Application.persistentDataPath + "/PlayerName.dat"))
            {
                playerName = reader.ReadLine();
            }
            File.Delete(Application.persistentDataPath + "/PlayerName.dat");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
            PauseAndUnpause(paused);
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
            UiManager.Instance.AddScore(((NpcScript)tank).Points);
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
    }

    void GameOver()
    {
        print("Game Over");
        print("Score: " + score);
        Time.timeScale = 0f;
        PlayerInfo[] highScores = new PlayerInfo[5];
        int index = 0;
        foreach (XmlNode node in highScoreXml.SelectNodes("//Player"))
        {
            print("?");
            highScores[index] = new PlayerInfo(int.Parse(node.Attributes[1].Value), node.Attributes[0].Value);
            index++;
        }
        for (int i = 0; i < highScores.Length; i++)
        {
            if (score > highScores[i].Score)
            {
                for(int j = highScores.Length - 2; j > i; j--)
                {
                    print("Before " + highScores[j].Score);
                    highScores[j] = highScores[j + 1];
                    print("After " + highScores[j].Score);
                }
                highScores[i] = new PlayerInfo(score, playerName);
                print("Player position: " + i);
                break;
            }
        }
        index = 0;
        foreach (XmlNode node in highScoreXml.SelectNodes("//Player"))
        {
            node.Attributes[0].InnerText = highScores[index].Name;
            node.Attributes[1].InnerText = highScores[index].Score.ToString();
            index++;
        }
        highScoreXml.Save(Application.streamingAssetsPath + "/HighScoreXML.xml");
        print(Application.streamingAssetsPath + "/HighScoreXML.xml");
        print(highScoreXml.InnerXml);
    }
}