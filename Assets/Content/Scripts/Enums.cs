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

public enum AudioType {
    Main,
    Music,
    SFX,
    Environment
}

public enum AntiAliasing {
    Disabled,
    MSAA,
    FXAA
}

public enum OptionsType {
    SliderTypeA,
    SliderTypeB,
    Dropdown
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
    FOV = 102,

    // Controls
    Sensitivity = 200,
    SprintToggle = 201,
}