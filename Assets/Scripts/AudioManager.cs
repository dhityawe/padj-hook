using UnityEngine;
using Hellmade.Sound;

public class AudioManager : MonoBehaviour
{
    public AudioClipStorage[] audioClips;

    // Make it static so it can be accessed from anywhere
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Don't destroy this object when loading new scenes
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void PlayMusic(int index, float volume = 1.0f, bool loop = true)
    {
        if (index < 0 || index >= audioClips.Length) return;

        AudioClip clip = audioClips[index].audioClip;
        if (clip == null) return;

        int audioID = EazySoundManager.PlayMusic(clip, volume, loop, false);
        audioClips[index].audio = EazySoundManager.GetAudio(audioID);
    }

    public void PlaySound(int index)
    {
        if (index < 0 || index >= audioClips.Length) return;
        
        AudioClip clip = audioClips[index].audioClip;
        if (clip == null) return;

        int audioID = EazySoundManager.PlaySound(clip, 1.0f);
        audioClips[index].audio = EazySoundManager.GetAudio(audioID);
    }

    public void PauseAudio(int index)
    {
        if (index < 0 || index >= audioClips.Length) return;
        if (audioClips[index].audio != null)
        {
            audioClips[index].audio.Pause();
        }
    }

    public void StopAudio(int index)
    {
        if (index < 0 || index >= audioClips.Length) return;
        if (audioClips[index].audio != null)
        {
            audioClips[index].audio.Stop();
        }
    }
}

[System.Serializable]
public struct AudioClipStorage
{
    public AudioClip audioClip;
    public Audio audio;
}