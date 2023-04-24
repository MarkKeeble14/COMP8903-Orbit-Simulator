using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum CameraFocus
{
    PLANET,
    ROCKET
}

public enum Objective
{
    TAKEOFF,
    LAND,
    ORBIT,
    ARM_ROCKET,
}

public enum ExtraEvents
{
    ALLOW_ROCKET_ARM
}

[System.Serializable]
public struct LevelObjective
{
    public Objective Objective;
    public List<DialogueSnippet> OnSuccessDialogue;
    public List<ExtraEvents> ExtraLevelEventsOnBegin;
}

public class GameManager : MonoBehaviour
{
    public static GameManager _Instance { get; private set; }

    [Header("Level")]
    [SerializeField] private LevelObjective[] levelObjectives;
    [SerializeField] private bool useLevelObjectives = true;
    [SerializeField] private List<DialogueSnippet> dialogueOnLevelStart;
    [SerializeField] private List<DialogueSnippet> dialogueOnLevelEnd;
    [SerializeField] private string nextScene;
    [SerializeField] private GameObjectStateData[] onStartLevelUIChanges;
    [SerializeField] private GameObjectStateData[] onEndLevelUIChanges;

    [Header("Skybox")]
    [SerializeField] private SkyboxBlender skyBoxBlender;
    [SerializeField] private float maxAltitudeToFinishBlend = 1000f;

    private int cameraFocusTracker;
    [SerializeField] private SerializableDictionary<CameraFocus, GameObjectStateData[]> cameraFocusDict = new SerializableDictionary<CameraFocus, GameObjectStateData[]>();

    [Header("Adjustable Settings")]
    [SerializeField] private float minAltitudeForTakeoff = 10.0f;
    [SerializeField] private bool allowImmediateArm = true;

    [Header("References")]
    [SerializeField] private NumStore altitude;
    [SerializeField] private Transform rocket;
    [SerializeField] private Transform planet;
    [SerializeField] private GameObject orbitalTarget;
    [SerializeField] private GameObject orbitalLine1;
    [SerializeField] private GameObject orbitalLine2;
    private GameObject[] currentOrbitalTargets;
    private GameObject[] orbitalTargetsOrder;

    [Header("Audio")]
    [SerializeField] private SimpleAudioClipContainer onSuccess;

    public bool RocketVelocityTooHighForImpact { get; set; }
    public bool RocketArmed { get; set; }
    public bool RocketExploded { get; set; }
    public bool AchievedOrbit { get; set; }

    private IEnumerator ProcessLevel()
    {
        foreach (GameObjectStateData data in onStartLevelUIChanges)
        {
            data.GameObject.SetActive(data.State == GameObjectState.ENABLED ? true : false);
        }

        // Play Dialogue on Level Start
        if (dialogueOnLevelStart.Count > 0)
        {
            DialogueManager._Instance.PlayDialogue(dialogueOnLevelStart);
        }

        Debug.Log("Processing Level Objectives");

        // For every objective in the level
        for (int i = 0; i < levelObjectives.Length; i++)
        {
            // Process that objective
            // Process just means run and wait for completion
            yield return StartCoroutine(ProcessLevelObjective(levelObjectives[i]));
        }

        Debug.Log("No More Level Objectives!");

        // Play Dialogue on Level End
        if (dialogueOnLevelEnd.Count > 0)
        {
            DialogueManager._Instance.PlayDialogue(dialogueOnLevelEnd);
        }

        // Set Helper text and such telling play they can proceed to true
        foreach (GameObjectStateData data in onEndLevelUIChanges)
        {
            data.GameObject.SetActive(data.State == GameObjectState.ENABLED ? true : false);
        }

        // Wait for player input before proceeding
        yield return new WaitUntil(() => Input.GetKey(KeyCode.Return) && !RocketExploded);

        TransitionManager._Instance.FadeOut(() => SceneManager.LoadScene(nextScene));
    }

    public GameObject[] SetNewOrbitalTarget()
    {
        AchievedOrbit = false;

        // Destroy previous targets
        Destroy(currentOrbitalTargets[0]);
        Destroy(currentOrbitalTargets[1]);
        Destroy(currentOrbitalTargets[2]);

        Vector3 toPlanet = rocket.position - planet.position;

        // Spawn 1st Target (On Rocket)
        currentOrbitalTargets[0] = Instantiate(orbitalTarget, rocket.position, Quaternion.identity);
        currentOrbitalTargets[0].name = "OrbitalTarget";

        // Spawn 2nd Target (Line)
        currentOrbitalTargets[1] = Instantiate(orbitalLine1, planet.position, Quaternion.identity);
        currentOrbitalTargets[1].transform.LookAt(rocket.position);
        currentOrbitalTargets[1].name = "OrbitalLine1";

        // Spawn 3rd Target (Line)
        currentOrbitalTargets[2] = Instantiate(orbitalLine2, planet.position, Quaternion.identity);
        currentOrbitalTargets[2].transform.LookAt(rocket.position);
        currentOrbitalTargets[2].name = "OrbitalLine2";

        orbitalTargetsOrder = new GameObject[5];
        orbitalTargetsOrder[0] = currentOrbitalTargets[0];
        orbitalTargetsOrder[1] = currentOrbitalTargets[2];
        orbitalTargetsOrder[2] = currentOrbitalTargets[1];
        orbitalTargetsOrder[3] = currentOrbitalTargets[2];
        orbitalTargetsOrder[4] = currentOrbitalTargets[0];

        return orbitalTargetsOrder;
    }

    private IEnumerator ProcessLevelObjective(LevelObjective levelObjective)
    {
        Debug.Log("Processing: " + levelObjective.Objective);
        foreach (ExtraEvents extraEvent in levelObjective.ExtraLevelEventsOnBegin)
        {
            switch (extraEvent)
            {
                case ExtraEvents.ALLOW_ROCKET_ARM:
                    Debug.Log("Extra Event: Allowed Rocket Arm");
                    RocketBlowUp._Instance.AllowArm = true;
                    break;
            }
        }

        // Change coroutine ran depending on objective
        switch (levelObjective.Objective)
        {
            case Objective.LAND:
                yield return StartCoroutine(ProcessLandObjective());
                break;
            case Objective.ORBIT:
                yield return StartCoroutine(ProcessOrbitObjective());
                break;
            case Objective.TAKEOFF:
                yield return StartCoroutine(ProcessTakeOffObjective());
                break;
            case Objective.ARM_ROCKET:
                yield return StartCoroutine(ProcessArmRocketObjective());
                break;
        }

        // Success!
        Debug.Log("Success!");
        DialogueManager._Instance.PlayDialogue(levelObjective.OnSuccessDialogue);
        onSuccess.PlayOneShot();
    }

    private IEnumerator ProcessOrbitObjective()
    {
        while (!AchievedOrbit)
        {
            yield return null;
        }
    }

    private IEnumerator ProcessTakeOffObjective()
    {
        yield return new WaitUntil(() => altitude.GetValue() > minAltitudeForTakeoff);
    }

    private IEnumerator ProcessArmRocketObjective()
    {
        yield return new WaitUntil(() => RocketArmed);
    }

    private IEnumerator ProcessLandObjective()
    {
        while (altitude.GetValue() > 0 || RocketVelocityTooHighForImpact)
        {
            yield return null;
        }
    }

    public void NextCameraFocus()
    {
        cameraFocusTracker++;
        if (cameraFocusTracker >= cameraFocusDict.Keys().Count)
            cameraFocusTracker = 0;
        SetCamera(cameraFocusDict.ToList()[cameraFocusTracker].Key);
    }

    public void SetCamera(CameraFocus focus)
    {
        SerializableKeyValuePair<CameraFocus, GameObjectStateData[]> kvp = cameraFocusDict.GetEntry(focus);
        foreach (GameObjectStateData obj in kvp.Value)
        {
            obj.GameObject.SetActive(obj.State == GameObjectState.ENABLED ? true : false);
        }
    }

    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(gameObject);
        }
        _Instance = this;

        ResetStores();
        SetCamera(CameraFocus.ROCKET);
        currentOrbitalTargets = new GameObject[3];
    }

    private void Start()
    {
        if (useLevelObjectives)
        {
            StartCoroutine(ProcessLevel());
        }
        else
        {
            DialogueManager._Instance.TryPlayOnStartDialogue();
        }

        // Only allow rocket to immediately arm if set to
        RocketBlowUp._Instance.AllowArm = allowImmediateArm;
    }

    private void Update()
    {
        float blendAmount = altitude.GetValue() / maxAltitudeToFinishBlend;
        if (blendAmount > 1) blendAmount = 1;
        skyBoxBlender.blend = blendAmount;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            NextCameraFocus();
        }
    }

    private void ResetStores()
    {
        GameStore[] stores = Resources.LoadAll<GameStore>("Stores");
        foreach (GameStore store in stores)
        {
            store.Reset();
        }
    }
}