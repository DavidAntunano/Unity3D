using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System;
using DG.Tweening;

public class SoundManager : Singleton<SoundManager> {

    protected SoundManager() { }

    #region Public variables
    [Serializable]
    public struct BackgroundClips {
        public string sceneName;
        public AudioClip backgroundMusic;
        public AudioMixerSnapshot audioMixerSnapshots;
    }

    public AudioSource MainBackgroundMusicSource;
    public AudioMixer mainAudioMixer;
    public BackgroundClips[] backgroundMusicClips;
    #endregion

    #region Private variables
    public int currentScene = 0;
    public AudioClip currentSceneBGMusic;
    private float[] weights = {0.0f, 1.0f};
    private float transitionTime = 0.1f;
    private AudioMixerSnapshot[] snapshotsToLoad = new AudioMixerSnapshot[2];
    #endregion

    #region GameManager variables & Main Menu settings variables
    private GameManager gameManager;

    private float playerSettingsFXVolume;
    private float playerSettingsBGVolume;

    private float currentBGMusic;
    private float currentFootstepsSound;
    private float currentVoicesSound;
    private float currentWeaponEffectsSound;
    private float currentOtherEfectsSound;
    #endregion

    void Start () {
        // Get current scene index
        currentScene = SceneManager.GetActiveScene().buildIndex -1;
		gameManager = GameManager.Instance;
        gameManager.SetSoundManager(tag);

        // Load scene Snapshot with the saved sound settings
        LoadSceneSnapshot();

        // Change volume settings based on the main menu options
        LoadMusicVolumeSettings();

        // Play background music
        PlayBackgroundMusic();
    }

    void LoadSceneSnapshot() {
        snapshotsToLoad[0] = backgroundMusicClips[0].audioMixerSnapshots;
        snapshotsToLoad[1] = backgroundMusicClips[currentScene].audioMixerSnapshots;

        mainAudioMixer.TransitionToSnapshots(snapshotsToLoad, weights, transitionTime);
    }

    void LoadMusicVolumeSettings() {
        // Get main menu settings from the game manager
        playerSettingsFXVolume = gameManager.preferences.effectsVolume;
        playerSettingsBGVolume = gameManager.preferences.musicVolume;
        
        // Get our volume settings from the game
        mainAudioMixer.GetFloat("BGMusic", out currentBGMusic);
        mainAudioMixer.GetFloat("Voices", out currentVoicesSound);
        mainAudioMixer.GetFloat("FootSteps", out currentFootstepsSound);
        mainAudioMixer.GetFloat("OtherEffects", out currentOtherEfectsSound);
        mainAudioMixer.GetFloat("WeaponEffects", out currentWeaponEffectsSound);
        
        // Put volume to the game using the options from main menu and our options
        mainAudioMixer.SetFloat("BGMusic", currentBGMusic * playerSettingsBGVolume);
        mainAudioMixer.SetFloat("Voices", currentVoicesSound * playerSettingsFXVolume);
        mainAudioMixer.SetFloat("FootSteps", currentFootstepsSound * playerSettingsFXVolume);
        mainAudioMixer.SetFloat("OtherEffects", currentOtherEfectsSound * playerSettingsFXVolume);
        mainAudioMixer.SetFloat("WeaponEffects", currentWeaponEffectsSound * playerSettingsFXVolume);
    }

    void PlayBackgroundMusic() {
        currentSceneBGMusic = backgroundMusicClips[currentScene].backgroundMusic;
        MainBackgroundMusicSource.clip = currentSceneBGMusic;
        MainBackgroundMusicSource.Play();
    }

    public void PitchBackgroundMusic(bool pausedGame) {
        if (pausedGame) {
            DOTween.To(value => MainBackgroundMusicSource.pitch = value, 1.0f, 0.0f, 1.0f).SetEase(Ease.Linear).SetUpdate(true);
        }
        else {
            DOTween.To(value => MainBackgroundMusicSource.pitch = value, 0.0f, 1.0f, 1.0f).SetEase(Ease.Linear).SetUpdate(true);
        }  
    }
}
