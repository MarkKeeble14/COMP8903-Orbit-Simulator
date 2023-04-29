using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager _Instance { get; set; }

    [SerializeField] private GameObjectStateData[] onPause;
    [SerializeField] private GameObjectStateData[] onResume;

    [SerializeField] private NumStore activeTimeScale;
    [SerializeField] private int[] possibleTimeScales;
    [SerializeField] private int startingTimeScaleIndex;

    [SerializeField] private bool allowUIInteractions = true;

    private bool timeScaleLocked;
    private bool paused;

    [SerializeField] private float uiMoveRate = 1.0f;
    [SerializeField] private ShownOrHiddenUIObjectPositioning reportingContainer;
    [SerializeField] private ShownOrHiddenUIObjectPositioning positioningContainer;
    [SerializeField] private ShownOrHiddenUIObjectPositioning engineContainer;
    [SerializeField] private ShownOrHiddenUIObjectPositioning inputContainer;
    [SerializeField] private ShownOrHiddenUIObjectPositioning levelInformationContainer;

    [System.Serializable]
    public struct ShownOrHiddenUIObjectPositioning
    {
        public Vector2 ShownPosition;
        public Vector2 HiddenPosition;
        public RectTransform UIObject;
        public bool Shown;
    }

    private bool canMoveUI;
    [SerializeField] private float delayCanMoveUI;

    [Header("Settings")]
    [SerializeField] private GameObject settingsUI;
    [SerializeField] private GameObject mainMenuUI;

    [Header("Audio")]
    [SerializeField] private Slider musicVolumeSlider;
    private string musicVolumeKey = "MusicVolume";
    [SerializeField] private float defaultMusicVolume = 0.8f;

    [SerializeField] private Slider sfxVolumeSlider;
    private string sfxVolumeKey = "SFXVolume";
    [SerializeField] private float defaultSFXVolume = 0.8f;

    [SerializeField] private AudioMixer mixer;

    [SerializeField] private PlanetLevel[] planetLevels;

    public static string totalStarsKey = "TotalStarsCollected";
    public static string GetLevelStarsCollectedKey(PlanetLevel level)
    {
        return "StarCollectedFrom" + level.ToString();
    }

    public static void SetPickedUpStarOnLevel(PlanetLevel level)
    {
        PlayerPrefs.SetInt(GetLevelStarsCollectedKey(level), 1);
        PlayerPrefs.SetInt(totalStarsKey, PlayerPrefs.GetInt(totalStarsKey) + 1);
        PlayerPrefs.Save();
    }

    public static bool GetHasPickedUpStarOnLevel(PlanetLevel level)
    {
        if (PlayerPrefs.HasKey(GetLevelStarsCollectedKey(level)))
        {
            return PlayerPrefs.GetInt(GetLevelStarsCollectedKey(level)) == 1;
        }
        return false;
    }

    public static int GetTotalStarsCollectedKey()
    {
        if (PlayerPrefs.HasKey(totalStarsKey))
        {
            return PlayerPrefs.GetInt(totalStarsKey);
        }
        return 0;
    }

    public int GetNumberOfLevels()
    {
        return planetLevels.Length;
    }

    private void Awake()
    {
        _Instance = this;
    }

    private void Start()
    {
        activeTimeScale.SetValue(possibleTimeScales[startingTimeScaleIndex]);

        if (!PlayerPrefs.HasKey(musicVolumeKey))
        {
            PlayerPrefs.SetFloat(musicVolumeKey, defaultMusicVolume);
            SetMusicVolume(defaultMusicVolume);
            musicVolumeSlider.value = defaultSFXVolume;
        }
        else
        {
            float musicVol = PlayerPrefs.GetFloat(musicVolumeKey);
            SetMusicVolume(musicVol);
            musicVolumeSlider.value = musicVol;
        }

        if (!PlayerPrefs.HasKey(sfxVolumeKey))
        {
            PlayerPrefs.SetFloat(sfxVolumeKey, defaultSFXVolume);
            SetSFXVolume(defaultSFXVolume);
            sfxVolumeSlider.value = defaultSFXVolume;
        }
        else
        {
            float sfxVol = PlayerPrefs.GetFloat(sfxVolumeKey);
            SetSFXVolume(sfxVol);
            sfxVolumeSlider.value = sfxVol;
        }
    }

    private void Update()
    {
        if (!allowUIInteractions)
        {
            return;
        }

        if (!paused && !timeScaleLocked)
        {
            Time.timeScale = activeTimeScale.GetValue();
        }

        if (!timeScaleLocked)
        {
            if (Input.GetKey(KeyCode.T))
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    activeTimeScale.SetValue(possibleTimeScales[0]);
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    activeTimeScale.SetValue(possibleTimeScales[1]);
                }
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    activeTimeScale.SetValue(possibleTimeScales[2]);
                }
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    activeTimeScale.SetValue(possibleTimeScales[3]);
                }
                if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    activeTimeScale.SetValue(possibleTimeScales[4]);
                }
                if (Input.GetKeyDown(KeyCode.Alpha6))
                {
                    activeTimeScale.SetValue(possibleTimeScales[5]);
                }
                if (Input.GetKeyDown(KeyCode.Alpha7))
                {
                    activeTimeScale.SetValue(possibleTimeScales[6]);
                }
                if (Input.GetKeyDown(KeyCode.Alpha8))
                {
                    activeTimeScale.SetValue(possibleTimeScales[7]);
                }
                if (Input.GetKeyDown(KeyCode.Alpha9))
                {
                    activeTimeScale.SetValue(possibleTimeScales[8]);
                }
                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    activeTimeScale.SetValue(possibleTimeScales[9]);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseState();
        }

        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                engineContainer.Shown = !engineContainer.Shown;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                inputContainer.Shown = !inputContainer.Shown;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                reportingContainer.Shown = !reportingContainer.Shown;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                positioningContainer.Shown = !positioningContainer.Shown;
            }
        }

        if (canMoveUI)
        {
            // Move UI
            MoveUIObjectPositioning(reportingContainer);
            MoveUIObjectPositioning(positioningContainer);
            MoveUIObjectPositioning(engineContainer);
            MoveUIObjectPositioning(inputContainer);
            MoveUIObjectPositioning(levelInformationContainer);
        }
        else
        {
            delayCanMoveUI -= Time.deltaTime;
            if (delayCanMoveUI <= 0)
                canMoveUI = true;
        }
    }

    private void MoveUIObjectPositioning(ShownOrHiddenUIObjectPositioning obj)
    {
        obj.UIObject.anchoredPosition = Vector2.Lerp(obj.UIObject.anchoredPosition, obj.Shown ? obj.ShownPosition : obj.HiddenPosition,
            Time.unscaledDeltaTime * uiMoveRate);
    }

    public void LockTimeScale(float lockedTimeScale)
    {
        Time.timeScale = lockedTimeScale;
        timeScaleLocked = true;
    }

    public void Restart()
    {
        TransitionManager._Instance.FadeOut(delegate
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }

    private void TogglePauseState()
    {
        if (paused)
        {
            Unpause();
        }
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        paused = true;

        Time.timeScale = 0;

        foreach (GameObjectStateData data in onPause)
        {
            data.GameObject.SetActive(data.State == GameObjectState.ENABLED ? true : false);
        }
    }

    public void Unpause()
    {
        paused = false;

        Time.timeScale = activeTimeScale.GetValue();

        foreach (GameObjectStateData data in onResume)
        {
            data.GameObject.SetActive(data.State == GameObjectState.ENABLED ? true : false);
        }
    }

    [ContextMenu("SetClearedAllLevels")]
    public void SetClearedAllLevels()
    {
        foreach (PlanetLevel planetLevel in planetLevels)
        {
            // Debug.Log("Set Key for: " + planetLevel.ToString() + " to 1");
            PlayerPrefs.SetInt(planetLevel.ToString(), 1);
            PlayerPrefs.SetInt(GetLevelStarsCollectedKey(planetLevel), 1);
        }
        PlayerPrefs.SetInt(totalStarsKey, planetLevels.Length);
        PlayerPrefs.Save();
        // Debug.Log("Saving PlayerPrefs");
    }

    [ContextMenu("SetUnclearedAllLevels")]
    public void SetUnclearAllLevels()
    {
        foreach (PlanetLevel planetLevel in planetLevels)
        {
            // Debug.Log("Set Key for: " + planetLevel.ToString() + " to 0");
            PlayerPrefs.SetInt(planetLevel.ToString(), 0);
            PlayerPrefs.SetInt(GetLevelStarsCollectedKey(planetLevel), 0);
        }
        PlayerPrefs.SetInt(totalStarsKey, 0);

        // First Level is always available
        PlayerPrefs.SetInt(PlanetLevel.LUBRITOV.ToString(), 1);
        PlayerPrefs.Save();
        // Debug.Log("Saving PlayerPrefs");
    }

    public void PlayGame()
    {
        SetUnclearAllLevels();
        TransitionManager._Instance.FadeOut(() => SceneManager.LoadScene("Tutorial"));
    }

    public void Simulate()
    {
        TransitionManager._Instance.FadeOut(() => SceneManager.LoadScene("Simulation"));
    }

    public void OpenSettings()
    {
        settingsUI.SetActive(true);
        if (mainMenuUI)
            mainMenuUI.SetActive(false);
    }

    public void CloseSettings()
    {
        settingsUI.SetActive(false);
        if (mainMenuUI)
            mainMenuUI.SetActive(true);
    }

    public void GoToMainMenu()
    {
        TransitionManager._Instance.FadeOut(() => SceneManager.LoadScene(0));
    }

    public void Exit()
    {
        TransitionManager._Instance.FadeOut(() => Application.Quit());
    }

    public void GoToLevelSelect()
    {
        TransitionManager._Instance.FadeOut(() => SceneManager.LoadScene(1));
    }

    public void GoToLevel(PlanetLevel planet)
    {
        TransitionManager._Instance.FadeOut(() => SceneManager.LoadScene(GameManager.ParsePlanetLevelToSceneString(planet)));
    }

    public void SetMusicVolume(float percent)
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(percent) * 20);
        PlayerPrefs.SetFloat(musicVolumeKey, percent);
    }

    public void SetSFXVolume(float percent)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(percent) * 20);
        PlayerPrefs.SetFloat(sfxVolumeKey, percent);
    }

    public void SavePlayerPrefs()
    {
        PlayerPrefs.Save();
    }
}
