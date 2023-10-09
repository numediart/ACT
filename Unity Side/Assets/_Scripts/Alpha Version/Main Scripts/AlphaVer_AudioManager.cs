using UnityEngine;

/// <summary>
/// This class manages audio playback in Unity. It includes functionality to play both music and sound effects.
/// </summary>
public class AlphaVer_AudioManager : MonoBehaviour
{
    
    /// <summary>
    /// A Transform object that represents the container for audio objects. Audio objects will be parented to this Transform.
    /// </summary>
    [Header("Instances")] public Transform AudioContainer;

    /// <summary>
    /// An array of SoundData objects. Each SoundData object contains a name and an array of AudioClips.
    /// </summary>
    public SoundData[] SoundDatas;

    /// <summary>
    /// A float value between 0 and 1 that represents the volume of the music.
    /// </summary>
    [Range(0, 1)]
    [Header("Options")] public float MusicVolume;

    /// <summary>
    /// A float value between 0 and 1 that represents the volume of the sound effects.
    /// </summary>
    [Range(0, 1)]
    public float SoundVolume;

    /// <summary>
    /// This nested class represents a single sound effect. It contains a name and an array of AudioClips.
    /// </summary>
    [System.Serializable]
    public class SoundData
    {
        /// <summary>
        /// A string representing the name of the sound effect.
        /// </summary>
        public string Name;

        /// <summary>
        /// An array of AudioClips that can be played for this sound effect.
        /// </summary>
        public AudioClip[] Clips;

        /// <summary>
        /// Returns a random AudioClip from the Clips array.
        /// </summary>
        public AudioClip GetRandomClip()
        {
            int rnd = Random.Range(0, Clips.Length);
            return Clips[rnd];
        }
    }

    /// <summary>
    /// Plays a music track with the given name.
    /// </summary>
    /// <param name="musicName">The name of the music track to play.</param>
    public void PlayMusic(string musicName)
    {
        SoundData sound = GetSound(musicName);

        if (sound != null)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(AudioContainer);
            go.name = musicName;
            AudioSource source = go.AddComponent<AudioSource>();
            source.clip = sound.GetRandomClip();
            source.volume = MusicVolume;
            source.loop = true;
            source.Play();
        }
    }

    /// <summary>
    /// Plays a sound effect with the given name.
    /// </summary>
    /// <param name="soundName">The name of the sound effect to play.</param>
    public void PlaySound(string soundName)
    {
        SoundData sound = GetSound(soundName);

        if (sound != null)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(AudioContainer);
            go.name = soundName;
            AudioSource source = go.AddComponent<AudioSource>();
            source.clip = sound.GetRandomClip();
            source.volume = SoundVolume;
            source.loop = false;
            source.Play();
            
            Destroy(go, source.clip.length);
        }
    }

    public void PlayAudioClip(AudioClip audioClip)
    {
        if (audioClip != null)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(AudioContainer);
            go.name = audioClip.name;
            AudioSource source = go.AddComponent<AudioSource>();
            source.clip = audioClip;
            source.volume = SoundVolume;
            source.loop = false;
            source.Play();
            
            Destroy(go, source.clip.length);
        }
    }

    /// <summary>
    /// Returns the SoundData object with the given name. If no SoundData object with the given name exists, returns null.
    /// </summary>
    /// <param name="audioName">The name of the SoundData object to retrieve.</param>
    private SoundData GetSound(string audioName)
    {
        foreach (var sound in SoundDatas)
        {
            if (sound.Name == audioName)
                return sound;
        }

        return null;
    }
}