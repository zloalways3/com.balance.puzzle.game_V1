using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchGameController : MonoBehaviour
{
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject tutorialPanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject privacyPolicyPanel;
    [SerializeField] GameObject quitConfirmationPanel;
    [SerializeField] GameObject levelSelectionPanel;
    [SerializeField] GameObject gameplayPanel;
    [SerializeField] GameObject victoryPanel;

    private bool isPolicyAccessedFromTutorial;
    private bool isSettingsAccessedFromGameplay;

    [SerializeField] TextMeshProUGUI victoryInfoText;
    [SerializeField] TextMeshProUGUI finalScoreText;
    [SerializeField] TextMeshProUGUI continueButtonText;

    private int playerPoints;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider soundEffectsSlider;
    private float musicVolumeLevel;
    private float soundEffectsVolume;

    [SerializeField] AudioSource musicAudioSource;
    [SerializeField] AudioSource soundEffectsAudioSource;

    [SerializeField] AudioClip buttonClickClip;
    [SerializeField] AudioClip ambientBackgroundClip;
    [SerializeField] GameObject winAnim;
    [SerializeField] GameTimer gameTimer;

    [SerializeField] MemoryMatchController memoryMatchController;
    [SerializeField] TextMeshProUGUI pointsText;

    private void Start()
    {
        var isTutor = PlayerPrefs.GetInt("tutor", 0);
        musicVolumeLevel = PlayerPrefs.GetFloat("musicVolume", 1);
        soundEffectsVolume = PlayerPrefs.GetFloat("soundVolume", 1);
        musicAudioSource.volume = musicVolumeLevel;
        musicVolumeSlider.value = musicVolumeLevel;
        soundEffectsSlider.value = soundEffectsVolume;
        musicAudioSource.clip = ambientBackgroundClip;
        musicAudioSource.Play();

        if (isTutor == 0)
        {
            tutorialPanel.SetActive(true);
            mainMenuPanel.SetActive(false);
        }
        else
        {
            mainMenuPanel.SetActive(true);
            tutorialPanel.SetActive(false);
        }

        settingsPanel.SetActive(false);
        privacyPolicyPanel.SetActive(false);
        quitConfirmationPanel.SetActive(false);
        levelSelectionPanel.SetActive(false);
        gameplayPanel.SetActive(false);
        victoryPanel.SetActive(false);
    }

    public void AdjustMusicVolume()
    {
        musicAudioSource.volume = musicVolumeSlider.value;
        PlayerPrefs.SetFloat("musicVolume", musicVolumeSlider.value);
        PlayerPrefs.Save();
    }

    public void AdjustSoundEffectsVolume()
    {
        soundEffectsVolume = soundEffectsSlider.value;
        PlayerPrefs.SetFloat("soundVolume", soundEffectsSlider.value);
        PlayerPrefs.Save();
    }

    public void PlayButtonClickSound()
    {
        soundEffectsAudioSource.PlayOneShot(buttonClickClip, soundEffectsVolume);
    }

    public void FinalizeTutorial()
    {
        PlayButtonClickSound();
        PlayerPrefs.SetInt("tutor", 1);
        PlayerPrefs.Save();
        tutorialPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void ShowPrivacyPolicyFromTutorial()
    {
        PlayButtonClickSound();
        privacyPolicyPanel.SetActive(true);
        tutorialPanel.SetActive(false);
        isPolicyAccessedFromTutorial = true;
    }

    public void HidePrivacyPolicy()
    {
        PlayButtonClickSound();
        privacyPolicyPanel.SetActive(false);
        if (isPolicyAccessedFromTutorial)
        {
            isPolicyAccessedFromTutorial = false;
            tutorialPanel.SetActive(true);
        }
        else
        {
            mainMenuPanel.SetActive(true);
        }
    }

    public void ShowPrivacyPolicyFromMainMenu()
    {
        PlayButtonClickSound();
        privacyPolicyPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void ShowSettingsFromMainMenu()
    {
        PlayButtonClickSound();
        settingsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void ShowSettingsFromGameplay()
    {
        PlayButtonClickSound();
        gameTimer.HaltTimer();
        isSettingsAccessedFromGameplay = true;
        settingsPanel.SetActive(true);
    }

    public void HideSettingsPanel()
    {
        PlayButtonClickSound();
        settingsPanel.SetActive(false);
        if (isSettingsAccessedFromGameplay)
        {
            isSettingsAccessedFromGameplay = false;
            gameTimer.ResumeTimer();
        }
        else
        {
            mainMenuPanel.SetActive(true);
        }
    }

    public void ShowLevelSelectionFromMainMenu()
    {
        PlayButtonClickSound();
        levelSelectionPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void ShowLevelSelectionFromVictory()
    {
        PlayButtonClickSound();
        levelSelectionPanel.SetActive(true);
        victoryPanel.SetActive(false);
    }

    public void ShowQuitConfirmation()
    {
        PlayButtonClickSound();
        quitConfirmationPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void HideQuitConfirmation()
    {
        PlayButtonClickSound();
        quitConfirmationPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void ExitApplication()
    {
        PlayButtonClickSound();
        Application.Quit();
    }

    public void ReturnToMainMenuFromLevelSelection()
    {
        PlayButtonClickSound();
        mainMenuPanel.SetActive(true);
        levelSelectionPanel.SetActive(false);
    }

    public void StartGameplay(int levelNumber)
    {
        PlayButtonClickSound();
        levelSelectionPanel.SetActive(false);
        gameplayPanel.SetActive(true);

        playerPoints = 0;
        pointsText.text = $"{playerPoints}";
        memoryMatchController.DefineGridSize(levelNumber + 4);
        gameTimer.InitializeMaximumTime(150 - levelNumber * 10);
        gameTimer.ResetTimer();
        memoryMatchController.InitializeGame();
    }

    public void ReturnToLevelSelectionFromGameplay()
    {
        PlayButtonClickSound();
        memoryMatchController.ConcludeGame();
        levelSelectionPanel.SetActive(true);
        gameplayPanel.SetActive(false);
    }

    public void ReturnToLevelSelectionFromVictory()
    {
        PlayButtonClickSound();
        levelSelectionPanel.SetActive(true);
        victoryPanel.SetActive(false);
    }

    public void ShowVictoryScreen()
    {
        gameTimer.HaltTimer();
        PlayButtonClickSound();
        gameplayPanel.SetActive(false);
        victoryPanel.SetActive(true);
        victoryInfoText.text = "WINNER";
        continueButtonText.text = "NEXT";
        finalScoreText.text = $"<#F9CF2E>SCORE\n<#FFFFFF>{playerPoints}";
        winAnim.SetActive(true);
    }

    public void ShowLoseScreen()
    {
        PlayButtonClickSound();
        gameplayPanel.SetActive(false);
        victoryPanel.SetActive(true);
        victoryInfoText.text = "LOSE";
        continueButtonText.text = "REPEAT";
        finalScoreText.text = $"OUT OF\nTIME";
        winAnim.SetActive(false);
    }

    public void AwardPoints()
    {
        playerPoints += 40;
        pointsText.text = $"{playerPoints}";
    }
}