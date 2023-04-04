using UnityEngine;

public enum CameraFocus
{
    PLANET,
    ROCKET
}

public class GameManager : MonoBehaviour
{
    public static GameManager _Instance { get; private set; }

    [Header("Skybox")]
    [SerializeField] private SkyboxBlender skyBoxBlender;
    [SerializeField] private float maxAltitudeToFinishBlend = 1000f;
    [SerializeField] private NumStore altitudeStore;

    private int cameraFocusTracker;
    [SerializeField] private CameraFocus currentCameraFocus;
    [SerializeField] private SerializableDictionary<CameraFocus, GameObjectStateData[]> cameraFocusDict = new SerializableDictionary<CameraFocus, GameObjectStateData[]>();

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
    }

    private void Update()
    {
        float blendAmount = altitudeStore.GetValue() / maxAltitudeToFinishBlend;
        if (blendAmount > 1) blendAmount = 1;
        skyBoxBlender.blend = blendAmount;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            NextCameraFocus();
        }
    }

    private void ResetStores()
    {
        NumStore[] stores = Resources.LoadAll<NumStore>("Stores");
        foreach (NumStore store in stores)
        {
            store.Reset();
        }
    }
}