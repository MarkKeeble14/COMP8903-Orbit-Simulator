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

    [SerializeField] private float uiMoveRate = 1.0f;
    [SerializeField] private ShownOrHiddenUIObjectPositioning reportingContainer;
    [SerializeField] private ShownOrHiddenUIObjectPositioning positioningContainer;
    [SerializeField] private ShownOrHiddenUIObjectPositioning engineContainer;
    [SerializeField] private ShownOrHiddenUIObjectPositioning inputContainer;

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
}
