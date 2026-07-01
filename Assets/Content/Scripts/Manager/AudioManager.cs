using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;

    [Header("Sources")]
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource sFX;
    [SerializeField] private AudioSource environmentA;
    [SerializeField] private AudioSource environmentB;

    [Header("Music")]
    [SerializeField] private int chance = 26;
    [SerializeField] private List<AudioClip> musicPlaylist = new();

    private float mainVolume = 1f;
    private float musicVolume = 1f;
    private float sfxVolume = 1f;
    private float environmentVolume = 1f;

    private float musicOwnVolume = 1f;
    private float environmentOwnVolume = 1f;

    private AudioSource currentEnvironment;
    private AudioSource nextEnvironment;

    private AudioClip previousClip;
    private Coroutine musicPlay;
    private Tween musicFadeTween;
    private Sequence environmentSequence;

    private float FinalMusicVolume => mainVolume * musicVolume * musicOwnVolume;
    private float FinalSFXVolume => mainVolume * sfxVolume;
    private float FinalEnvironmentVolume => mainVolume * environmentVolume * environmentOwnVolume;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        currentEnvironment = environmentA;
        nextEnvironment = environmentB;

        environmentA.loop = true;
        environmentB.loop = true;
    }

    private void Start() {
        StartCoroutine(OnLaunch());
    }

    private IEnumerator OnLaunch() {
        yield return new WaitUntil(() =>
            SaveManager.Instance != null && SaveManager.Instance.init);

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
        RefreshMusicVolume();
    }

    public void SetSFXVolume(float volume) {
        sfxVolume = volume;
        SaveManager.Instance.sfxVolume = volume;
        RefreshSFXVolume();
    }

    public void SetEnvironmentVolume(float volume) {
        environmentVolume = volume;
        SaveManager.Instance.enviromentVolume = volume;
        RefreshEnvironmentVolume();
    }

    private void ApplyVolumes() {
        RefreshMusicVolume();
        RefreshSFXVolume();
        RefreshEnvironmentVolume();
    }

    private void RefreshMusicVolume() {
        music.volume = FinalMusicVolume;
    }

    private void RefreshSFXVolume() {
        sFX.volume = FinalSFXVolume;
    }

    private void RefreshEnvironmentVolume() {
        if (currentEnvironment != null)
            currentEnvironment.volume = FinalEnvironmentVolume;

        if (nextEnvironment != null && nextEnvironment.isPlaying)
            nextEnvironment.volume = FinalEnvironmentVolume;
    }

    public void PlaySFX(AudioClip clip) {
        if (clip == null) return;
        sFX.PlayOneShot(clip);
    }

    public void PlayEnvironment(AudioClip clip, float clipVolume) {
        environmentOwnVolume = clipVolume;

        environmentSequence?.Kill();

        if (clip == null) {
            environmentSequence = DOTween.Sequence();

            environmentSequence.Append(currentEnvironment.DOFade(0f, 0.5f));
            environmentSequence.Join(nextEnvironment.DOFade(0f, 0.5f));

            environmentSequence.OnComplete(() => {
                currentEnvironment.Stop();
                currentEnvironment.clip = null;

                nextEnvironment.Stop();
                nextEnvironment.clip = null;
            });

            return;
        }

        if (currentEnvironment.clip == clip && currentEnvironment.isPlaying) {
            RefreshEnvironmentVolume();
            return;
        }

        nextEnvironment.Stop();
        nextEnvironment.clip = clip;
        nextEnvironment.loop = true;
        nextEnvironment.volume = 0f;
        nextEnvironment.Play();

        environmentSequence = DOTween.Sequence();

        environmentSequence.Append(currentEnvironment.DOFade(0f, 7f));
        environmentSequence.Join(nextEnvironment.DOFade(FinalEnvironmentVolume, 7f));

        environmentSequence.OnComplete(() => {
            currentEnvironment.Stop();
            currentEnvironment.volume = 0f;

            (currentEnvironment, nextEnvironment) = (nextEnvironment, currentEnvironment);
        });
    }

    private IEnumerator PlayMusic(AudioClip clip) {
        if (clip == null) {
            musicPlay = null;
            yield break;
        }

        music.clip = clip;
        previousClip = clip;

        musicOwnVolume = 0f;
        RefreshMusicVolume();

        music.Play();

        musicFadeTween = DOTween.To(
            () => musicOwnVolume,
            x => {
                musicOwnVolume = x;
                RefreshMusicVolume();
            },
            1f,
            0.5f);

        yield return musicFadeTween.WaitForCompletion();

        yield return new WaitForSeconds(Mathf.Max(0f, clip.length - 1f));

        musicFadeTween = DOTween.To(
            () => musicOwnVolume,
            x => {
                musicOwnVolume = x;
                RefreshMusicVolume();
            },
            0f,
            0.5f);

        yield return musicFadeTween.WaitForCompletion();

        music.Stop();

        musicOwnVolume = 1f;
        RefreshMusicVolume();

        musicPlay = null;
    }

    private IEnumerator RunPlaylist() {
        while (true) {
            if (musicPlay == null &&
                !music.isPlaying &&
                !Mathf.Approximately(FinalMusicVolume, 0f) &&
                musicPlaylist.Count > 1 &&
                Random.Range(0, chance) == 1) {
                List<AudioClip> playable = musicPlaylist
                    .Where(x => x != previousClip)
                    .ToList();

                if (playable.Count == 0)
                    playable = new List<AudioClip>(musicPlaylist);

                AudioClip nextClip = playable[Random.Range(0, playable.Count)];
                musicPlay = StartCoroutine(PlayMusic(nextClip));
            }

            yield return new WaitForSeconds(10f);
        }
    }
}