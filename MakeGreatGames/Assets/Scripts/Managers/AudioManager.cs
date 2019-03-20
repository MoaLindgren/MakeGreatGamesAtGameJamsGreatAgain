using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    int sfxVoices, musicVoices;     //Specify how many voices should be reserved for SFX and music AudioSources

    [SerializeField]
    GameObject sfxAudioPrefab, musicAudioPrefab;       //Different prefabs for music and SFX sounds so they can use different groups in the AudioMixer

    int currentSFXIndex = 0, currentMusicIndex = 0;     //Keep track of which object to use

    AudioSource[] sfxSources, musicSources;

    Dictionary<AudioSource, Coroutine> soundsInUse = new Dictionary<AudioSource, Coroutine>();   //Store the coroutines in a Dictionary to easily stop the coroutines when re-pooling the GameObject

    Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();      //Store all available AudioClips and access them by name

    static AudioManager instance;       //Static object reference for easy access to the active AudioManager

    public static AudioManager Instance
    {
        get { return instance; }
    }

    private void Awake()            //Make sure exactly one AudioManager is in use
    {
        if (instance != null && instance != this)
            Destroy(this);
        instance = this;
        Object[] loadedClips = Resources.LoadAll("AudioClips", typeof(AudioClip));
        foreach (Object o in loadedClips)
        {
            clips.Add(o.name, o as AudioClip);
        }
        sfxSources = new AudioSource[sfxVoices];
        musicSources = new AudioSource[musicVoices];
        for (int i = 0; i < sfxSources.Length; i++)         //Spawn pools
        {
            sfxSources[i] = Instantiate(sfxAudioPrefab, transform).GetComponent<AudioSource>();
        }
        for (int i = 0; i < musicSources.Length; i++)
        {
            musicSources[i] = Instantiate(musicAudioPrefab, transform).GetComponent<AudioSource>();
        }
    }

    public AudioSource SpawnSound(string clip, Transform t, bool stationarySound, bool looping, bool isMusic, float volume)       //Determines which AudioSource to use and returns that AudioSource to the caller
    {
        if (!clips.ContainsKey(clip))
        {
            print("Clip \"" + clip + "\" not found, please check spelling.");
            return null;
        }
        AudioSource[] arrToUse = isMusic ? musicSources : sfxSources;
        int index = isMusic ? currentMusicIndex : currentSFXIndex, sourcesTried = 0;
        AudioSource sourceToUse = arrToUse[index];
        while (soundsInUse.ContainsKey(sourceToUse))
        {
            index = (index + 1) % arrToUse.Length;
            sourceToUse = arrToUse[index];
            sourcesTried++;
            if (sourcesTried >= arrToUse.Length)        //If there are no free AudioSources we check if we can reuse one currently playing
            {
                foreach(AudioSource playingSound in soundsInUse.Keys)
                {
                    if (playingSound.clip.name == clip)
                    {
                        ReturnSource(playingSound);
                        return SpawnSound(clip, t, stationarySound, looping, isMusic, volume);      //If an AudioSource with the same clip is already playing, we stop that clip and reuse the AudioSource for the new sound
                    }
                }
                return null;        //No free AudioSources for that soundtype, sorry
            }
        }
        sourceToUse.loop = looping;
        currentMusicIndex = isMusic ? index : currentMusicIndex;
        currentSFXIndex = isMusic ? currentSFXIndex : index;
        float playTime = looping ? Mathf.Infinity : clips[clip].length;
        sourceToUse.clip = clips[clip];
        Coroutine soundTimer = StartCoroutine(SoundPlayTime(playTime, sourceToUse));
        soundsInUse.Add(sourceToUse, soundTimer);
        sourceToUse.transform.SetParent(t, stationarySound);
        sourceToUse.transform.localPosition = Vector3.zero;
        sourceToUse.volume = volume;
        if (looping)
            sourceToUse.Play();
        else
            sourceToUse.PlayOneShot(clips[clip], volume);
        return sourceToUse;
    }

    public void ReturnSource(AudioSource source)        //Returns an AudioSource to the pool when it's finished playing. May be called by SoundPlayTime or externally
    {
        if (source == null)
        {
            return;
        }
        source.volume = 0f;
        source.Stop();
        if (soundsInUse.ContainsKey(source))
        {
            StopCoroutine(soundsInUse[source]);
            soundsInUse.Remove(source);
        }
        source.transform.SetParent(transform, true);
        source.transform.localPosition = Vector3.zero;
    }

    IEnumerator SoundPlayTime(float playTime, AudioSource source)   //Keeps track of when to repool an AudioSource
    {
        yield return new WaitForSeconds(playTime);
        ReturnSource(source);
    }

    /*
    IEnumerator DuckSounds(string soundsToDuck)     //Ducks sounds when several AudioSources play the same clip to keep the audio clean
    {

    }
    */
}