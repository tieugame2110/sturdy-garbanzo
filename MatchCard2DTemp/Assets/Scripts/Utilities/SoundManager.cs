using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [System.Serializable]
    public class Sound
    {
        public AudioClip clip;
        [HideInInspector]
        public int simultaneousPlayCount = 0;
    }
    [Header("Max number allowed of same sounds playing together")]
    public int maxSimultaneousSounds = 13;

    public Sound backgroundMusic;
    public Sound clickSound;
    public Sound revealCard;
    public Sound matchedSound;
    public Sound mismatchSound;
    public Sound endGameSound;

    private AudioSource audioSource;
    private const string MUSIC_PREF_KEY = "MusicPreference";
    private const int MUSIC_OFF = 0;
    private const int MUSIC_ON = 1;

    void Awake()
    {
        if (SoundManager.Instance)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
        audioSource = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        //SetMute(IsMuted());
    }
    public void PlayMusic(Sound music, bool loop = true)
    {
        if (IsMusicOff())
        {
            return;
        }

        audioSource.clip = music.clip;
        audioSource.loop = loop;
        audioSource.Play();
    }
    public void PlaySound(Sound sound, bool autoScaleVolume = true, float maxVolumeScale = 1f)
    {
        StartCoroutine(CRPlaySound(sound, autoScaleVolume, maxVolumeScale));
    }

    IEnumerator CRPlaySound(Sound sound, bool autoScaleVolume = true, float maxVolumeScale = 1f)
    {
        if (sound.simultaneousPlayCount >= maxSimultaneousSounds)
        {
            yield break;
        }

        sound.simultaneousPlayCount++;

        float vol = maxVolumeScale;

        // Scale down volume of same sound played subsequently
        if (autoScaleVolume && sound.simultaneousPlayCount > 0)
        {
            vol = vol / (float)(sound.simultaneousPlayCount);
        }

        audioSource.PlayOneShot(sound.clip, vol);

        // Wait til the sound almost finishes playing then reduce play count
        float delay = sound.clip.length * 0.7f;

        yield return new WaitForSeconds(delay);

        sound.simultaneousPlayCount--;
    }
    public void Stop()
    {
        audioSource.Stop();
    }
    public bool IsMusicOff()
    {
        return (PlayerPrefs.GetInt(MUSIC_PREF_KEY, MUSIC_ON) == MUSIC_OFF);
    }
    void SetMute(bool isMuted)
    {
        audioSource.mute = isMuted;
    }
    public void SetVolume(float _value)
    {
        audioSource.volume = _value;
    }
}
