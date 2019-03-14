using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    #region Field

    #region SerializedVariables

    [SerializeField]
    GameObject gameOverScreen, playerPrefab;

    [SerializeField]
    Transform playerSpawn;

    [SerializeField]
    Text scoreText;

    [SerializeField]
    PoolScript enemyPool, projectilePool, missilePool, minePool;

    #endregion

    #region PrivateVariables

    XmlDocument highScoreXml = new XmlDocument();

    int score = 0;

    Camera cam;

    static GameManager instance;

    string playerName = "Air Guitar Elemental";

    bool paused = false;

    #endregion

    #region Properties

    public int Score
    {
        set { score += value; }
    }

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

    public PoolScript EnemyPool
    {
        get { return enemyPool; }
    }

    public PoolScript ProjectilePool
    {
        get { return projectilePool; }
    }

    public PoolScript MissilePool
    {
        get { return missilePool; }
    }

    public PoolScript MinePool
    {
        get { return minePool; }
    }

    #endregion

    #endregion

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        instance = this;
        SceneManager.sceneLoaded += ResetTimescale;
        Cursor.visible = false;
        highScoreXml.Load(Application.streamingAssetsPath + "/HighScoreXML.xml");
        Instantiate(playerPrefab, playerSpawn.position, Quaternion.identity);
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

    public void RestartGame()
    {
        using (StreamWriter writer = File.CreateText(Application.persistentDataPath + "/PlayerName.dat"))
        {
            writer.WriteLine(playerName);
        }
    }

    public void ResetTimescale(Scene scene, LoadSceneMode lsm)
    {
        Time.timeScale = 1;
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
        Cursor.visible = pause;
        if (pause)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
        UiManager.Instance.Pause(pause);
    }

    void GameOver()
    {
        Cursor.visible = true;
        scoreText.text = score.ToString();
        gameOverScreen.SetActive(true);
        CameraShaker.Instance.StopShaking();
        Cursor.visible = true;
        Time.timeScale = 0f;
        PlayerInfo[] highScores = new PlayerInfo[5];
        int index = 0;
        foreach (XmlNode node in highScoreXml.SelectNodes("//Player"))
        {
            highScores[index] = new PlayerInfo(int.Parse(node.Attributes[1].Value), node.Attributes[0].Value);
            index++;
        }
        for (int i = 0; i < highScores.Length; i++)
        {
            if (score > highScores[i].Score)
            {
                for (int j = highScores.Length - 2; j > i; j--)
                {
                    highScores[j] = highScores[j - 1];
                }
                highScores[i] = new PlayerInfo(score, playerName);
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
    }
}