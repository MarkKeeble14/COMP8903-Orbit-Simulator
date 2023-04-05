using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager _Instance { get; set; }

    [SerializeField] private GameObjectStateData[] onPause;
    [SerializeField] private GameObjectStateData[] onResume;

    [SerializeField] private NumStore activeTimeScale;
    [SerializeField] private int[] possibleTimeScales;
    [SerializeField] private int startingTimeScaleIndex;

    private bool timeScaleLocked;
    private bool paused;

    private void Awake()
    {
        _Instance = this;
    }

    private void Start()
    {
        activeTimeScale.SetValue(possibleTimeScales[startingTimeScaleIndex]);
    }

    private void Update()
    {
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
}
