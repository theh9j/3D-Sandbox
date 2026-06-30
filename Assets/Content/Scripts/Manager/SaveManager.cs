using NUnit.Framework;
using System;
using System.Collections.Generic;
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
    [HideInInspector] public int antiAlias;

    //Camera
    [HideInInspector] public float sensitivity;
    [HideInInspector] public int fov;

    [HideInInspector] public bool sprintToggle;

    [HideInInspector] public Key moveFW;
    [HideInInspector] public Key moveBW;
    [HideInInspector] public Key moveL;
    [HideInInspector] public Key moveR;
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
        antiAlias = PlayerPrefs.GetInt("AntiAliasing", 0);

        sensitivity = PlayerPrefs.GetFloat("Sensitivity", 1.5f);
        fov = PlayerPrefs.GetInt("FOV", 70);

        sprintToggle = PlayerPrefs.GetInt("SprintToggle", 0) == 1;

        interact = GetKeybind(Key.E, "Interact");
        sprint = GetKeybind(Key.LeftShift, "Sprint");

        moveFW = GetKeybind(Key.W, "Foward");
        moveBW = GetKeybind(Key.S, "Backward");
        moveL = GetKeybind(Key.A, "Left");
        moveR = GetKeybind(Key.D, "Right");

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
        PlayerPrefs.SetInt("AntiAliasing", antiAlias);

        //CONTROLS
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
        PlayerPrefs.SetInt("FOV", fov);

        PlayerPrefs.SetInt("SprintToggle", sprintToggle ? 1 : 0);

        PlayerPrefs.SetString("Interact", interact.ToString());
        PlayerPrefs.SetString("Sprint", sprint.ToString());

        PlayerPrefs.SetString("Forward", moveFW.ToString());
        PlayerPrefs.SetString("Backward", moveBW.ToString());
        PlayerPrefs.SetString("Left", moveL.ToString());
        PlayerPrefs.SetString("Right", moveR.ToString());

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
        antiAlias = 0;
        sensitivity = 1.5f;
        fov = 70;

        sprintToggle = false;

        mainVolume = 1f;
        musicVolume = 1f;
        sfxVolume = 1f;
        enviromentVolume = 1f;
    }

    private void FirstLaunchKB() {
        moveFW = Key.W;
        moveBW = Key.S;
        moveL = Key.A;
        moveR = Key.D;

        interact = Key.E;
        sprint = Key.LeftShift;
    }
}
