using System;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    [SerializeField] private bool resetData = false;

    //Player Settings
    public float sensitivity;
    public float fov;

    //Audio
    public float musicVolume;
    public bool musicMuted;

    public float sfxVolume;
    public bool sfxMuted;
    

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    void Start() {
        if (resetData) PlayerPrefs.DeleteAll();
        if (!PlayerPrefs.HasKey("FirstLaunch")) { 
            FirstLaunch();
            return;
        }

        sensitivity = PlayerPrefs.GetFloat("Sensitivity", .15f);
        fov = PlayerPrefs.GetFloat("FOV", .7f);

        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        musicMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1 ? true : false;

        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        sfxMuted = PlayerPrefs.GetInt("SFXMuted", 0) == 1 ? true : false;
    }

    private void FirstLaunch() {
        sensitivity = .15f;
        fov = .7f;

        musicVolume = 1f;
        musicMuted = false;

        sfxVolume = 1f;
        sfxMuted = false;

        PlayerPrefs.SetInt("FirstLaunch", 1);
        PlayerPrefs.Save();
    }

    private void Save() {
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
        PlayerPrefs.SetFloat("FOV", fov);

        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetInt("MusicMuted", musicMuted ? 1 : 0);

        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetInt("SFXMuted", sfxMuted ? 1 : 0);

        PlayerPrefs.Save();

    }

    void OnApplicationPause(bool pause) {
        if (pause) Save();
    }

    void OnApplicationQuit() {
        Save();
    }

}
