using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.Audio;

public class SettingsInitializer : MonoBehaviour
{
    [SerializeField]
    AudioMixer mixer;

    XmlDocument settingsXML = new XmlDocument();

    XmlNode levelsNode;

    void Start()
    {
        if (mixer == null)
            return;
        settingsXML.Load(Application.streamingAssetsPath + "/SettingsXML.xml");
        levelsNode = settingsXML.SelectSingleNode("/Settings/Audio/Levels");
        mixer.SetFloat("Master", float.Parse(levelsNode.Attributes["Master"].Value));
        mixer.SetFloat("Music", float.Parse(levelsNode.Attributes["Music"].Value));
        mixer.SetFloat("SFX", float.Parse(levelsNode.Attributes["SFX"].Value));
        Destroy(this);
    }
}