using System.Collections.Generic;
using UnityEngine;

public class AudioSourceManager : MonoBehaviour
{
    #region Singleton

    private static AudioSourceManager _instance = null;
    public static AudioSourceManager instance
    {
        get
        {
            if (_instance == null)
                CreateInstance();

            return _instance;
        }
    }

    private static void CreateInstance()
    {
        // Create a new gameobject to contain the instance script
        GameObject g = new GameObject("AudioSourceManager");
        Object.DontDestroyOnLoad(g);
        g.hideFlags = HideFlags.HideAndDontSave;

        // Attach an instance
        _instance = g.AddComponent<AudioSourceManager>();
    }

    #endregion

    #region Audio Channel Settings

    public enum AudioChannel { Master, Effects };   // TODO: Add more channels, maybe ambient, music, etc.

    // Prefs keys
    const string _masterVolumePrefsKey = "MasterVolume";
    const string _effectsVolumePrefsKey = "EffectsVolume";

    // Prefs values
    private float _masterVolume = 1.0f;
    private float _effectsVolume = 1.0f;

    #endregion

    #region Audio Player Tracking

    private List<AudioPlayer> _audioPlayers = new List<AudioPlayer>();

    #endregion

    private void Awake()
    {
        // Load audio channel values from PlayerPrefs
        LoadAudioPrefs();

        // Save the prefs again, in case this is the first-time launch and the prefs do not yet exist
        SaveAudioPrefs();
    }

    private void OnValidate()
    {
        SaveAudioPrefs();
    }

    private void Update()
    {
        foreach (AudioPlayer p in _audioPlayers)
        {
            p.source.pitch = Time.timeScale * p.defaultPitch;
        }
    }

    private void LoadAudioPrefs()
    {
        // Read master volume
        if (PlayerPrefs.HasKey(_masterVolumePrefsKey))
            _masterVolume = PlayerPrefs.GetFloat(_masterVolumePrefsKey);

        // Read effects volume
        if (PlayerPrefs.HasKey(_effectsVolumePrefsKey))
            _effectsVolume = PlayerPrefs.GetFloat(_effectsVolumePrefsKey);
    }

    private void SaveAudioPrefs()
    {
        PlayerPrefs.SetFloat(_masterVolumePrefsKey, _masterVolume);
        PlayerPrefs.SetFloat(_effectsVolumePrefsKey, _effectsVolume);

        PlayerPrefs.Save();
    }

    /// <summary>
    /// Registers the specified AudioPlayer to the manager.
    /// </summary>
    public void RegisterPlayer(AudioPlayer p)
    {
        if (p)
            _audioPlayers.Add(p);
    }
    
    /// <returns>Returns the value for the AudioChannel c, scaled by Master Volume.</returns>
    public float GetChannelValue(AudioChannel c)
    {
        float value = GetChannelValueUnscaled(c);

        if (c != AudioChannel.Master)
            value *= _masterVolume;

        return value;
    }
    
    /// <returns>Returns the unscaled value for the AudioChannel c.</returns>
    public float GetChannelValueUnscaled(AudioChannel c)
    {
        switch (c)
        {
            case AudioChannel.Master:
                return _masterVolume;
            case AudioChannel.Effects:
                return _effectsVolume;

            default:
                return 0.0f;
        }
    }
    
}
