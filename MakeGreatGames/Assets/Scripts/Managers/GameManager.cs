﻿using System;
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
    PoolScript enemyPool, projectilePool, missilePool, minePool, crackPool;

    #endregion

    #region PrivateVariables

    XmlDocument highScoreXml = new XmlDocument(), statsXML = new XmlDocument();

    int score = 0;

    Camera cam;

    static GameManager instance;

    string playerName = "Air Guitar Elemental";

    bool paused = false;

    bool gameRunning = false;

    #endregion

    #region Properties

    public int Score
    {
        set { score += value; }
    }

    public bool GameRunning
    {
        get { return gameRunning; }
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

    public PoolScript CrackPool
    {
        get { return crackPool; }
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
        statsXML.Load(Application.streamingAssetsPath + "/TankStatsXML.xml");
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

    private void Start()
    {
        AudioSource mainThemeAudio = AudioManager.Instance.SpawnSound("BANDITANKSThemeMaybe", transform, true, true, true, 1f);
        gameRunning = true;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Start"))
        {
            paused = !paused;
            PauseAndUnpause(paused);
        }
    }

    public T GetTankStat<T>(string baseOrTower, string objectName, string statName) where T : IComparable
    {
        foreach (XmlNode node in statsXML.SelectNodes("//" + baseOrTower))
        {
            if (node.Attributes["Name"].Value == objectName)
            {
                T returnVar;
                try
                {
                    returnVar = (T)Convert.ChangeType(node.Attributes[statName].Value, typeof(T));  //Value successfully converted
                }
                catch
                {
                    print("Value could not be converted, returning default value");
                    returnVar = (T)Convert.ChangeType(statsXML.SelectSingleNode("/" + baseOrTower + "s/Default").Attributes[statName].Value, typeof(T));    //Value could not be converted, returning default value
                }
                return returnVar;
            }
        }
        print(objectName + ": Item " + statName + " not found, returning default value");
        return (T)Convert.ChangeType(statsXML.SelectSingleNode("/Components/" + baseOrTower + "s/Default").Attributes[statName].Value, typeof(T));     //Item not found, returning default value
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
            UIManager.Instance.AddScore(((NpcScript)tank).Points);
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
        UIManager.Instance.Pause(pause);
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