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
    [SerializeField] private int chance = 26;
    [SerializeField] private List<AudioClip> musicPlaylist = new();
    private float mainVolume;
    private float musicVolume;
    private float sfxVolume;
    private float environmentVolume;

    private AudioClip previousClip;
    private Coroutine musicPlay;



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
        yield return new WaitUntil(() =>
            SaveManager.Instance != null && SaveManager.Instance.init
        );

        mainVolume = SaveManager.Instance.mainVolume;
        musicVolume = SaveManager.Instance.musicVolume;
        sfxVolume = SaveManager.Instance.sfxVolume;
        environmentVolume = SaveManager.Instance.enviromentVolume;

        ApplyVolumes();

        StartCoroutine(RunPlaylist());
    }

    public void SetMainVolume(float volume) {
        mainVolume = volume;
        SaveManager.Instance.mainVolume = volume;
        ApplyVolumes();
    }

    public void SetMusicVolume(float volume) {
        musicVolume = volume;
        SaveManager.Instance.musicVolume = volume;
        ApplyVolumes();
    }

    public void SetSFXVolume(float volume) {
        sfxVolume = volume;
        SaveManager.Instance.sfxVolume = volume;
        ApplyVolumes();
    }

    public void SetEnvironmentVolume(float volume) {
        environmentVolume = volume;
        SaveManager.Instance.enviromentVolume = volume;
        ApplyVolumes();
    }

    private void ApplyVolumes() {
        music.volume = musicVolume * mainVolume;
        sFX.volume = sfxVolume * mainVolume;
        environment.volume = environmentVolume * mainVolume;
    }

    public void PlaySFX(AudioClip clip) {
        sFX.PlayOneShot(clip);
    }

    private IEnumerator PlayMusic(AudioClip clip) {
        music.clip = clip;
        previousClip = clip;
        float initialVolume = music.volume;
        music.volume = 0f;
        music.Play();

        yield return music.DOFade(initialVolume, .5f).WaitForCompletion();

        yield return new WaitForSeconds(Mathf.Max(0f, clip.length - 1f));

        yield return music.DOFade(0f, .5f).WaitForCompletion();

        music.Stop();
        music.volume = initialVolume;
        musicPlay = null;
    }

    private IEnumerator RunPlaylist() {
        while (true) {
            if (musicPlay == null &&
                !music.isPlaying &&
                !Mathf.Approximately(music.volume, 0f) &&
                !Mathf.Approximately(mainVolume, 0f) &&
                musicPlaylist.Count > 1 &&
                Random.Range(0, chance) == 1) { 

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
