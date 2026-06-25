using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    [SerializeField] private bool resetData = false;

    //Camera
    [HideInInspector] public float sensitivity;
    [HideInInspector] public float fov;

    //Keybind
    [HideInInspector] public Key interact;

    //Audio
    [HideInInspector] public float musicVolume;
    [HideInInspector] public bool musicMuted;

    [HideInInspector] public float sfxVolume;
    [HideInInspector] public bool sfxMuted;

    [HideInInspector] public bool init = false;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    void Start() {
        if (resetData) PlayerPrefs.DeleteKey("FirstLaunch");
        if (!PlayerPrefs.HasKey("FirstLaunch")) { 
            FirstLaunch();
            init = true;
            return;
        }

        sensitivity = PlayerPrefs.GetFloat("Sensitivity", .15f);
        fov = PlayerPrefs.GetFloat("FOV", 70f);

        interact = GetKeybind(Key.E, "Interact");

        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        musicMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1 ? true : false;

        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        sfxMuted = PlayerPrefs.GetInt("SFXMuted", 0) == 1 ? true : false;
        init = true;
    }

    private void FirstLaunch() {
        FirstLaunchGeneric();
        FirstLaunchKB();

        PlayerPrefs.SetInt("FirstLaunch", 1);
        PlayerPrefs.Save();
    }

    private void Save() {
        //CAMERA
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
        PlayerPrefs.SetFloat("FOV", fov);

        //KEYBIND
        PlayerPrefs.SetString("Interact", interact.ToString());


        //AUDIO
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

    //HELPER

    private Key GetKeybind(Key keyType, string key) {
        if (!Enum.TryParse(PlayerPrefs.GetString(key), out Key convert)) {
            return keyType;
        }
        return convert;
    }

    private void FirstLaunchGeneric() {
        sensitivity = .15f;
        fov = 70f;

        musicVolume = 1f;
        musicMuted = false;

        sfxVolume = 1f;
        sfxMuted = false;
    }

    private void FirstLaunchKB() {
        interact = Key.E;
    }

    
}
