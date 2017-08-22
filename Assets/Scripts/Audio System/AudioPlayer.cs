using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{

    #region Audio Clip Variables

    [System.Serializable]
    private class NamedAudioClip
    {
        public string _name = "";
        public AudioClip _clip = null;
    }

    [SerializeField, HideInInspector]   private List<NamedAudioClip> _clips;

    #endregion

    #region References
    
    private AudioSourceManager _managerRef = null;
    private AudioSource _source;
    public AudioSource source
    {
        get { return _source; }
    }

    #endregion

    #region Audio Variables

    [SerializeField]    private AudioSourceManager.AudioChannel _channel = AudioSourceManager.AudioChannel.Effects;
    public AudioSourceManager.AudioChannel channel
    {
        get { return _channel; }
        set { _channel = value; }
    }

    [SerializeField]    private bool _timeScaleInfluencesPlayback = true;
    public bool timeScaleInfluencesPlayback
    {
        get { return _timeScaleInfluencesPlayback; }
    }

    private float _defaultPitch = 1.0f;
    private float _defaultVolume = 1.0f;

    public float defaultPitch
    {
        get { return _defaultPitch; }
    }
    public float defaultVolume
    {
        get { return _defaultVolume; }
    }

    #endregion

    void Awake()
    {
        // Get ref to AudioSource component
        _source = GetComponent<AudioSource>();

        // Subscribe to manager
        _managerRef = AudioSourceManager.instance;
        _managerRef.RegisterPlayer(this);

        // Set defaults
        _defaultPitch = _source.pitch;
        _defaultVolume = _source.volume;
    }

    private AudioClip GetAudioClip(string name)
    {
        NamedAudioClip clip = _clips.Find((NamedAudioClip c) => { return c._name == name; } );

        if (clip == null)
        {
            Debug.LogWarning("No AudioClip attached with name: " + name, this);
            return null;
        }

        return clip._clip;
    }

    /// <summary>Plays the clip with name clipName.</summary>
    public void PlayOneShot(string clipName)
    {
        AudioClip clip = GetAudioClip(clipName);

        if (clip)
        {
            float shotVolume = (_defaultVolume / _source.volume) * _managerRef.GetChannelValueUnscaled(_channel);

            _source.PlayOneShot(clip, shotVolume);
        }
    }

    /// <summary>Plays the clip with name clipName with a delay specified in seconds</summary>
    public void PlayDelayed(string clipName, float delay)
    {
        AudioClip clip = GetAudioClip(clipName);
        if (clip)
        {
            _source.clip = clip;
            _source.volume = _defaultVolume * _managerRef.GetChannelValue(_channel);
            _source.PlayDelayed(delay);
        }
    }

    /// <summary>Pauses the playing clip.</summary>
    public void Pause()
    {
        _source.Pause();
    }

    /// <summary>Unpauses the paused playback of the attached AudioSource.</summary>
    public void UnPause()
    {
        _source.UnPause();
    }

    /// <summary>Stops playing the clip.</summary>
    public void Stop()
    {
        _source.Stop();
    }

    /// <summary>Stops playing all sounds, including the current playing clip and all clips started via the PlayOneShot method.</summary>
    public void StopAll()
    {
        _source.enabled = false;
        _source.enabled = true;
    }
}
