using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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

[Serializable]
public class ListWrapper<T>
{
    public List<T> Wrapped;
}

[System.Serializable]
public struct LevelObjective
{
    public Objective Objective;
    public List<DialogueSnippet> OnSuccessDialogue;
    public List<ExtraEvents> ExtraLevelEventsOnBegin;
}

public enum PlanetLevel
{
    LUBRITOV,
    SITHUENIDES,
    KILLUINUS,
    OIPHUS,
    XELVOTH,
    KALLURN,
    HUIVIS,
    GAGREUPRA,
    COBAGUA,
    CAITERA,
    RANNINDA
}

public class GameManager : MonoBehaviour
{
    public static GameManager _Instance { get; private set; }
    [SerializeField] private bool useObjectPooler;
    [SerializeField] private bool useLevelObjectives = true;

    [SerializeField] private bool enableDevCheatInputs;

    [SerializeField] private bool showIntroductionText = true;

    [SerializeField] private PlanetLevel planetLevel;
    [SerializeField] private bool usePlanetLevelOrder = true;
    [SerializeField] private string forcedNextSceneName;
    [SerializeField] private SerializableDictionary<PlanetLevel, PlanetLevel> planetLevelOrderDict = new SerializableDictionary<PlanetLevel, PlanetLevel>();

    [Header("Level Settings")]
    [SerializeField] private ScientificNotation levelPlanetMass;
    [SerializeField] private float levelMaxThrust;
    [SerializeField] private float levelFuelCapacity;

    [Header("Level Data")]
    [SerializeField] private LevelObjective[] levelObjectives;
    [SerializeField] private List<DialogueSnippet> dialogueOnLevelStart;
    [SerializeField] private List<DialogueSnippet> dialogueOnLevelEnd;
    [SerializeField] private GameObjectStateData[] onStartLevelUIChanges;
    [SerializeField] private GameObjectStateData[] onEndLevelUIChanges;

    [Header("Skybox")]
    [SerializeField] private SkyboxBlender skyBoxBlender;
    [SerializeField] private float maxAltitudeToFinishBlend = 1000f;

    [Header("Simulation Data")]
    [SerializeField] private ScientificNotation simulatingPlanetMass;
    [SerializeField] private float simulatingMaxThrust;
    [SerializeField] private float simulatingFuelCapacity;

    [Header("Adjustable Settings")]
    [SerializeField] private float minAltitudeForTakeoff = 10.0f;
    [SerializeField] private bool allowImmediateArm = true;

    private int cameraFocusTracker;
    [SerializeField] private SerializableDictionary<CameraFocus, GameObjectStateData[]> cameraFocusDict = new SerializableDictionary<CameraFocus, GameObjectStateData[]>();

    private bool hasSpawnedOrbitalTargets;
    // Playing dialogue on successful orbit
    private bool lastFrameWasOrbiting;
    private bool playedAchievedOrbitText;
    [SerializeField] private List<ListWrapper<DialogueSnippet>> achievedOrbitTextOptions = new List<ListWrapper<DialogueSnippet>>();

    [Header("Stars")]
    [SerializeField] private bool spawnStar = true;
    [SerializeField] private StarPickup star;
    [SerializeField] private Vector2 minMaxStarSpawnHeight;
    private StarPickup spawnedStar;

    [Header("References")]
    [SerializeField] private NumStore altitude;
    [SerializeField] private ScientificNotationStore planetMass;
    [SerializeField] private NumStore maxThrust;
    [SerializeField] private NumStore fuelCapacity;

    [SerializeField] private Transform rocket;
    [SerializeField] private Transform planet;

    [SerializeField] private Renderer planetRenderer;

    [SerializeField] private RocketEngine rocketEngine;
    [SerializeField] private GameObject orbitalTarget;
    [SerializeField] private GameObject orbitalLine1;
    [SerializeField] private GameObject orbitalLine2;
    private GameObject[] currentOrbitalTargets;
    private List<GameObject> orbitalTargetsOrder = new List<GameObject>();
    [SerializeField] private TextMeshProUGUI popupText;
    [SerializeField] private Transform canvasTransform;

    [Header("Audio")]
    [SerializeField] private SimpleAudioClipContainer onSuccess;

    public bool RocketVelocityTooHighForImpact { get; set; }
    public bool RocketArmed { get; set; }
    public bool RocketExploded { get; set; }
    public bool AchievedOrbit { get; set; }
    public string CurrentPlanet
    {
        get => ParsePlanetLevelToSceneString(planetLevel);
    }

    [Header("Random Planet Generation")]
    [SerializeField] private List<Texture2D> albedoMapOptions;
    [SerializeField] private List<Texture2D> normalMapOptions;
    [SerializeField] private Vector2 minMaxNormalMapImpact = new Vector2(0, 10);
    [SerializeField] private List<Texture2D> heightMapOptions;
    [SerializeField] private Vector2 minMaxHeightMapImpact = new Vector2(0.01f, 0.25f);
    [SerializeField] private List<Texture2D> emissionMapOptions;
    [SerializeField] private Vector2 chanceToBeEmissive;
    [SerializeField] private Vector2 minMaxEmissionIntensity;
    [SerializeField] private int minColorComponentValue = 64;
    [SerializeField] private Vector2 minMaxTextureTiling = new Vector2(1, 5);
    private Material planetMaterial;

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

        // Debug.Log("Processing Level Objectives");

        // For every objective in the level
        for (int i = 0; i < levelObjectives.Length; i++)
        {
            // Process that objective
            // Process just means run and wait for completion
            yield return StartCoroutine(ProcessLevelObjective(levelObjectives[i]));
        }

        // Debug.Log("No More Level Objectives!");;

        PlayerPrefs.SetInt(planetLevel.ToString(), 1);
        if (planetLevelOrderDict.ContainsKey(planetLevel))
        {
            // Debug.Log("Not On Last Level");
            PlayerPrefs.SetInt(planetLevelOrderDict[planetLevel].ToString(), 1);
        }
        else
        {
            // Debug.Log("On Last Level");
        }
        PlayerPrefs.Save();

        if (StarPickup._Instance != null)
        {
            if (StarPickup._Instance.HasBeenPickedUp)
            {
                UIManager.SetPickedUpStarOnLevel(planetLevel);
            }
            else
            {
                spawnedStar.Fade();
            }
        }

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

        bool restarting = false;
        bool nextLevel = false;
        while (!RocketExploded)
        {
            if (Input.GetKey(KeyCode.Return))
            {
                nextLevel = true;
                break;
            }

            if (Input.GetKey(KeyCode.R))
            {
                restarting = true;
                break;
            }

            yield return null;
        }

        if (nextLevel)
        {
            if (usePlanetLevelOrder)
            {
                TransitionManager._Instance.FadeOut(delegate
                {
                    SceneManager.LoadScene(ParsePlanetLevelToSceneString(planetLevelOrderDict[planetLevel]));
                });
            }
            else
            {
                TransitionManager._Instance.FadeOut(() => SceneManager.LoadScene(forcedNextSceneName));
            }
        }
        else if (restarting)
        {
            TransitionManager._Instance.FadeOut(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
        }

    }

    public List<GameObject> SetNewOrbitalTarget()
    {
        playedAchievedOrbitText = false;
        AchievedOrbit = false;

        if (StarPickup._Instance != null && StarPickup._Instance.HasBeenPickedUp)
        {
            StarPickup._Instance.BringBack();
        }

        // Release/Destroy Previous Targets
        if (hasSpawnedOrbitalTargets)
        {
            // Destroy previous targets
            if (useObjectPooler)
            {
                ObjectPooler._Instance.ReleaseSimpleObject(SimpleObjectType.ORBITAL_TARGET_SPHERE, currentOrbitalTargets[0]);
                ObjectPooler._Instance.ReleaseSimpleObject(SimpleObjectType.ORBITAL_TARGET_FACE_Z, currentOrbitalTargets[1]);
                ObjectPooler._Instance.ReleaseSimpleObject(SimpleObjectType.ORBITAL_TARGET_FACE_Y, currentOrbitalTargets[2]);
            }
            else
            {
                Destroy(currentOrbitalTargets[0]);
                Destroy(currentOrbitalTargets[1]);
                Destroy(currentOrbitalTargets[2]);
            }
        }

        // Spawn/Get New Targets
        if (useObjectPooler)
        {
            currentOrbitalTargets[0] = ObjectPooler._Instance.GetSimpleObject(SimpleObjectType.ORBITAL_TARGET_SPHERE);
            currentOrbitalTargets[1] = ObjectPooler._Instance.GetSimpleObject(SimpleObjectType.ORBITAL_TARGET_FACE_Z);
            currentOrbitalTargets[2] = ObjectPooler._Instance.GetSimpleObject(SimpleObjectType.ORBITAL_TARGET_FACE_Y);
        }
        else
        {
            currentOrbitalTargets[0] = Instantiate(orbitalTarget, rocket.position, Quaternion.identity);
            currentOrbitalTargets[1] = Instantiate(orbitalLine1, planet.position, Quaternion.identity);
            currentOrbitalTargets[2] = Instantiate(orbitalLine2, planet.position, Quaternion.identity);
        }

        // Transform Targets Appropriately
        currentOrbitalTargets[0].transform.position = rocket.position;
        currentOrbitalTargets[1].transform.LookAt(rocket.position);
        currentOrbitalTargets[2].transform.LookAt(rocket.position);

        // Now we know to destroy targets next time we run this
        hasSpawnedOrbitalTargets = true;

        // Fill the list in the order the player must pass through each target
        orbitalTargetsOrder.Clear();
        orbitalTargetsOrder.Add(currentOrbitalTargets[0]);
        orbitalTargetsOrder.Add(currentOrbitalTargets[2]);
        orbitalTargetsOrder.Add(currentOrbitalTargets[1]);
        orbitalTargetsOrder.Add(currentOrbitalTargets[2]);
        orbitalTargetsOrder.Add(currentOrbitalTargets[0]);

        return orbitalTargetsOrder;
    }

    private IEnumerator ProcessLevelObjective(LevelObjective levelObjective)
    {
        // Debug.Log("Processing: " + levelObjective.Objective);
        foreach (ExtraEvents extraEvent in levelObjective.ExtraLevelEventsOnBegin)
        {
            switch (extraEvent)
            {
                case ExtraEvents.ALLOW_ROCKET_ARM:
                    // Debug.Log("Extra Event: Allowed Rocket Arm");
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
        // Debug.Log("Success!");
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
        foreach (PlanetNameDisplay planetNameDisplay in FindObjectsOfType<PlanetNameDisplay>(true))
        {
            planetNameDisplay.Set();
        }

        planetMaterial = planetRenderer.material;

        if (showIntroductionText)
        {
            TextMeshProUGUI spawned = Instantiate(popupText, canvasTransform);
            spawned.text = "Welcome to " + ParsePlanetLevelToSceneString(planetLevel);
        }

        if (spawnStar)
        {
            // Spawn star if the player has not collected the star on this level
            if (PlayerPrefs.GetInt(UIManager.GetLevelStarsCollectedKey(planetLevel)) == 0)
            {
                SpawnStar();
            }
        }
    }

    private void SpawnStar()
    {
        Vector3 spawnPos = new Vector3(planet.position.x, planet.position.y - planet.localScale.y - RandomHelper.RandomFloat(minMaxStarSpawnHeight), 0);
        spawnedStar = Instantiate(star, spawnPos, Quaternion.identity);
    }

    private void Start()
    {
        if (useLevelObjectives)
        {
            // Playing
            // Debug.Log("Setting Level Variables");
            planetMass.SetValue(levelPlanetMass);
            rocketEngine.SetMaximumThrust(levelMaxThrust);
            rocketEngine.SetInitialFuelCapacity(levelFuelCapacity, true);

            StartCoroutine(ProcessLevel());
        }
        else
        {
            // Simulating
            // Debug.Log("Setting Simulation Variables");
            planetMass.SetValue(simulatingPlanetMass);
            rocketEngine.SetMaximumThrust(simulatingMaxThrust);
            rocketEngine.SetInitialFuelCapacity(simulatingFuelCapacity, true);

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

        // if we're simulating
        if (!useLevelObjectives)
        {
            if (!lastFrameWasOrbiting && AchievedOrbit && !playedAchievedOrbitText)
            {
                DialogueManager._Instance.PlayDialogue(RandomHelper.GetRandomFromList(achievedOrbitTextOptions).Wrapped);
                playedAchievedOrbitText = true;
                onSuccess.PlayOneShot();
            }
        }
        lastFrameWasOrbiting = AchievedOrbit;

        if (!enableDevCheatInputs) return;
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (usePlanetLevelOrder)
            {
                TransitionManager._Instance.FadeOut(delegate
                {
                    SceneManager.LoadScene(ParsePlanetLevelToSceneString(planetLevelOrderDict[planetLevel]));
                });
            }
            else
            {
                TransitionManager._Instance.FadeOut(() => SceneManager.LoadScene(forcedNextSceneName));
            }
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

    public static string FirstCharToUpper(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }
        return $"{char.ToUpper(input[0])}{input[1..]}";
    }

    public static string ParsePlanetLevelToSceneString(PlanetLevel planetName)
    {
        return FirstCharToUpper(planetName.ToString().ToLower());
    }

    public void RandomizePlanet()
    {
        Color albedoColor = RandomHelper.GetRandomColor(minColorComponentValue) / 255;
        planetMaterial.SetTexture("_BaseMap", RandomHelper.GetRandomFromList(albedoMapOptions));
        planetMaterial.SetColor("_BaseColor", albedoColor);

        planetMaterial.SetTexture("_NormalMap", RandomHelper.GetRandomFromList(normalMapOptions));
        planetMaterial.SetFloat("_BumpScale", RandomHelper.RandomFloat(minMaxNormalMapImpact));

        if (RandomHelper.EvaluateChanceTo(chanceToBeEmissive))
        {
            planetMaterial.EnableKeyword("_EMISSION");
            planetMaterial.SetTexture("_EmissionMap", RandomHelper.GetRandomFromList(emissionMapOptions));
            planetMaterial.SetColor("_EmissionColor", albedoColor * RandomHelper.RandomFloat(minMaxEmissionIntensity));
        }
        else
        {
            planetMaterial.DisableKeyword("_EMISSION");
        }

        planetMaterial.SetTexture("_ParallaxMap", RandomHelper.GetRandomFromList(heightMapOptions));
        planetMaterial.SetFloat("_Parallax", RandomHelper.RandomFloat(minMaxHeightMapImpact));

        planetMaterial.mainTextureScale = RandomHelper.RandomVector2(minMaxTextureTiling);
    }
}