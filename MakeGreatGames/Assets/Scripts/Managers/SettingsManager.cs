using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField]
    AudioMixer mixer;

    [SerializeField]
    Slider masterSlider, musicSlider, sfxSlider;

    XmlDocument settingsXML = new XmlDocument();

    XmlNode levelsNode;

    static SettingsManager instance;

    public static SettingsManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        instance = this;
        settingsXML.Load(Application.streamingAssetsPath + "/SettingsXML.xml");
        levelsNode = settingsXML.SelectSingleNode("/Settings/Audio/Levels");
        ResetSettings();
    }

    public void ChangeMasterVolume(float value)
    {
        if (mixer == null)
            return;
        mixer.SetFloat("Master", value);
    }

    public void ChangeMusicVolume(float value)
    {
        if (mixer == null)
            return;
        mixer.SetFloat("Music", value);
    }

    public void ChangeSFXVolume(float value)
    {
        if (mixer == null)
            return;
        mixer.SetFloat("SFX", value);
    }

    public void SetColorblindMode(bool activate)
    {
        //Gotta get shader working for this :c
    }

    public void SaveSettings()
    {
        if (mixer == null)
            return;
        float masterLevel, musicLevel, sfxLevel;
        if (mixer.GetFloat("Master", out masterLevel))
            levelsNode.Attributes["Master"].Value = masterLevel.ToString();
        if (mixer.GetFloat("Music", out musicLevel))
            levelsNode.Attributes["Music"].Value = musicLevel.ToString();
        if (mixer.GetFloat("SFX", out sfxLevel))
            levelsNode.Attributes["SFX"].Value = sfxLevel.ToString();
        settingsXML.Save(Application.streamingAssetsPath + "/SettingsXML.xml");
    }

    public void ResetSettings()
    {
        if (mixer == null)
            return;
        masterSlider.value = float.Parse(levelsNode.Attributes["Master"].Value);
        musicSlider.value = float.Parse(levelsNode.Attributes["Music"].Value);
        sfxSlider.value = float.Parse(levelsNode.Attributes["SFX"].Value);
    }
}