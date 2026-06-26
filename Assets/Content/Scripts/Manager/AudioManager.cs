using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource sFX;
    [SerializeField] private AudioSource environment;
    [SerializeField] private List<AudioClip> musicPlaylist = new();
    private AudioClip previousClip;
    private Coroutine musicPlay;
    public float MusicVolume { get; private set; }



    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start() {
        StartCoroutine(OnLaunch());
    }

    private IEnumerator OnLaunch() {
        yield return new WaitUntil(() => SaveManager.Instance != null && SaveManager.Instance.init);
        if (SaveManager.Instance != null) {
            SetMusicVolume(SaveManager.Instance.musicVolume);
            SetSFXVolume(SaveManager.Instance.sfxVolume);
            SetEnvironmentVolume(SaveManager.Instance.enviromentVolume);

            StartCoroutine(RunPlaylist());
        }
    }

    public void SetMusicVolume(float volume) {
        music.volume = volume;
        MusicVolume = volume;
        SaveManager.Instance.musicVolume = music.volume;
    }

    public void SetSFXVolume(float volume) {
        sFX.volume = volume;
        SaveManager.Instance.sfxVolume = sFX.volume;
    }

    public void SetEnvironmentVolume(float volume) {
        environment.volume = volume;
        SaveManager.Instance.enviromentVolume = environment.volume;
    }

    public void PlaySFX(AudioClip clip) {
        sFX.PlayOneShot(clip);
    }

    private IEnumerator PlayMusic(AudioClip clip) {
        music.clip = clip;
        previousClip = clip;
        music.volume = 0f;
        music.Play();

        yield return music.DOFade(MusicVolume, .5f).WaitForCompletion();

        yield return new WaitForSeconds(Mathf.Max(0f, clip.length - 1f));

        yield return music.DOFade(0f, .5f).WaitForCompletion();

        music.Stop();
        music.volume = MusicVolume;
        musicPlay = null;
    }

    private IEnumerator RunPlaylist() {
        while (true) {
            if (musicPlay == null &&
                !music.isPlaying &&
                !Mathf.Approximately(MusicVolume, 0f) &&
                musicPlaylist.Count > 1 &&
                Random.Range(0, 26) == 5) {

                List<AudioClip> playable = musicPlaylist
                    .Where(x => x != previousClip)
                    .ToList();

                if (playable.Count == 0)
                    playable = new List<AudioClip>(musicPlaylist);

                AudioClip next = playable[Random.Range(0, playable.Count)];
                musicPlay = StartCoroutine(PlayMusic(next));
            }

            yield return new WaitForSeconds(10f);
        }
    }
}
