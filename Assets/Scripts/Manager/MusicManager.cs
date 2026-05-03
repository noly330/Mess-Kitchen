using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";
    public static MusicManager Instance;
    private AudioSource _audioSource;
    private float _volume = 0.3f;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, 0.3f);
        _audioSource.volume = _volume;
    }
    public void ChangeVolume()
    {
        _volume += 0.1f;
        if (_volume > 1.02f)
        {
            _volume = 0f;
        }
        _audioSource.volume = _volume;
        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, _volume);
        PlayerPrefs.Save();
    }
    public float GetVolume()
    {
        return _volume;
    }

}
