using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    [SerializeField] private bool resetData = false;

    //Player
    [HideInInspector] public float stamina; //Stamina == 999 -> Reset to Max

    //Settings
    [HideInInspector] public int fps;
    [HideInInspector] public bool vsync;
    [HideInInspector] public string antiAlias;

    //Camera
    [HideInInspector] public float sensitivity;
    [HideInInspector] public float fov;

    [HideInInspector] public bool sprintToggle;

    [HideInInspector] public Key interact;
    [HideInInspector] public Key sprint;

    //Audio
    [HideInInspector] public float mainVolume;
    [HideInInspector] public float musicVolume;
    [HideInInspector] public float sfxVolume;
    [HideInInspector] public float enviromentVolume;

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

        stamina = PlayerPrefs.GetFloat("Stamina", 999f);

        fps = PlayerPrefs.GetInt("FPS", 60);
        vsync = PlayerPrefs.GetInt("VSync", 1) == 1;
        antiAlias = PlayerPrefs.GetString("AntiAliasing", "Disabled");

        sensitivity = PlayerPrefs.GetFloat("Sensitivity", .15f);
        fov = PlayerPrefs.GetFloat("FOV", 70f);

        sprintToggle = PlayerPrefs.GetInt("SprintToggle", 0) == 1;

        interact = GetKeybind(Key.E, "Interact");
        sprint = GetKeybind(Key.LeftShift, "Sprint");

        mainVolume = PlayerPrefs.GetFloat("MainVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        enviromentVolume = PlayerPrefs.GetFloat("EnvironmentVolume", 1f);

        init = true;
    }

    private void FirstLaunch() {
        FirstLaunchGeneric();
        FirstLaunchKB();

        PlayerPrefs.SetInt("FirstLaunch", 1);
        PlayerPrefs.Save();
    }

    private void Save() {
        //PLAYER
        PlayerPrefs.SetFloat("Stamina", stamina);

        //SETTINGS
        PlayerPrefs.SetInt("FPS", fps);
        PlayerPrefs.SetInt("VSync", vsync ? 1 : 0);
        PlayerPrefs.SetString("AntiAliasing", antiAlias);

        //KEYSETTINGS
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
        PlayerPrefs.SetFloat("FOV", fov);

        PlayerPrefs.SetInt("SprintToggle", sprintToggle ? 1 : 0);

        PlayerPrefs.SetString("Interact", interact.ToString());
        PlayerPrefs.SetString("Sprint", sprint.ToString());

        //AUDIO
        PlayerPrefs.SetFloat("MainVolume", mainVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat("EnvironmentVolume", enviromentVolume);

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
        stamina = 999f;

        fps = 60;
        vsync = false;
        antiAlias = "Disabled";
        sensitivity = .15f;
        fov = 70f;

        sprintToggle = false;

        mainVolume = 1f;
        musicVolume = 1f;
        sfxVolume = 1f;
        enviromentVolume = 1f;
    }

    private void FirstLaunchKB() {
        interact = Key.E;
        sprint = Key.LeftShift;
    }

    
}
