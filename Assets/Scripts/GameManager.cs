using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager _Instance { get; private set; }

    [Header("Skybox")]
    [SerializeField] private SkyboxBlender skyBoxBlender;
    [SerializeField] private float maxAltitudeToFinishBlend = 1000f;
    [SerializeField] private NumStore altitudeStore;

    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(gameObject);
        }
        _Instance = this;

        ResetStores();
    }

    private void Update()
    {
        float blendAmount = altitudeStore.GetValue() / maxAltitudeToFinishBlend;
        if (blendAmount > 1) blendAmount = 1;
        skyBoxBlender.blend = blendAmount;
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