using UnityEngine;

public enum InputType {
    None,
    Computer,
    Touch
}

public enum Modes {
    Normal,
    Build,
    Spectate
}

public enum UIState {
    Gameplay,
    SemiPause,
    Pause,
}

public enum OptionsType {
    SliderTypeA,
    SliderTypeB,
    Dropdown,
    InputKey
}

public enum OptionIDs {
    // Audio
    MainVolume = 0,
    MusicVolume = 1,
    SFXVolume = 2,
    EnvironmentVolume = 3,

    // Graphics
    FPSLimit = 100,
    VSync = 101,
    AntiAliasing = 102,

    // Controls
    FOV = 200,
    Sensitivity = 201,
    SprintToggle = 202,

    //Keybinds
    MoveFW,
    MoveBW,
    MoveL,
    MoveR,

    Interact,
    Sprint,
}