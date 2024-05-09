using Unity.VisualScripting;
using UnityEngine;

public class ManagerSounds : MonoBehaviour
{
    public static ManagerSounds Instance { get; private set; }

    [SerializeField] private AudioSource _sourceMusic;
    [SerializeField] private AudioSource _sourceSfx;

    [SerializeField] private AudioClip _musicMenu;
    [SerializeField] private AudioClip _musicGameplay;
    
    [SerializeField] private AudioClip _sfxCollect;

    public AudioClip MusicMenu => _musicMenu;
    public AudioClip MusicGameplay => _musicGameplay;
    public AudioClip SfxCollect => _sfxCollect;

    private const float LowPitchRange = .95f;
    private const float HighPitchRange = 1.05f;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }
    
    public void PlayMusic(AudioClip clip)
    {
        _sourceMusic.clip = clip;
        MusicSourcePlay();
    }
    
    public void PlaySingle(AudioClip clip, bool randomizePitch = false)
    {
        SfxPlay(clip, randomizePitch);
    }

    private void MusicSourcePlay()
    {
        //if (Settings.music > 0) //In a full implementation this would use our sound settings to avoid playing the sound or not 
        {
            _sourceMusic.Play();
        }
    }
    
    private void SfxPlay(AudioClip clip, bool randomizePitch = false)
    {
        // if (Settings.sfx > 0) //In a full implementation this would use our sound settings to avoid playing the sound or not
        {
            _sourceSfx.pitch = randomizePitch ? Random.Range(LowPitchRange, HighPitchRange) : 1f;
            _sourceSfx.PlayOneShot(clip);
        }
    }
}
