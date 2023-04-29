using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler _Instance { get; private set; }

    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(_Instance.gameObject);
        }

        // Set instance
        _Instance = this;
    }

    [SerializeField]
    private SerializableDictionary<SimpleObjectType, GameObject> simpleObjectPrefabs
        = new SerializableDictionary<SimpleObjectType, GameObject>();
    private Dictionary<SimpleObjectType, ObjectPool<GameObject>> simpleObjectPools = new Dictionary<SimpleObjectType, ObjectPool<GameObject>>();

    public static ObjectPool<Flame> _FlamePool;
    [SerializeField] private Flame flamePrefab;
    [SerializeField] private int numInitFlamePoolWith = 100;

    public static ObjectPool<Smoke> _SmokePool;
    [SerializeField] private Smoke smokePrefab;
    [SerializeField] private int numInitSmokePoolWith = 500;

    private void CreateFlamePool()
    {
        _FlamePool = new ObjectPool<Flame>(() =>
        {
            return Instantiate(flamePrefab, transform);
        }, flame =>
        {
            flame.gameObject.SetActive(true);
        }, flame =>
        {
            flame.gameObject.SetActive(false);
        }, flame =>
        {
            Destroy(flame.gameObject);
        }, true, numInitFlamePoolWith);
    }

    private void CreateSmokePool()
    {
        _SmokePool = new ObjectPool<Smoke>(() =>
        {
            return Instantiate(smokePrefab, transform);
        }, smoke =>
        {
            smoke.gameObject.SetActive(true);
        }, smoke =>
        {
            smoke.gameObject.SetActive(false);
        }, smoke =>
        {
            Destroy(smoke.gameObject);
        }, true, numInitSmokePoolWith);
    }

    private void Start()
    {
        CreateFlamePool();
        CreateSmokePool();
    }

    public ObjectPool<GameObject> GetSimpleObjectPool(SimpleObjectType type)
    {
        if (simpleObjectPools.ContainsKey(type))
        {
            return simpleObjectPools[type];
        }
        else
        {
            GameObject obj = simpleObjectPrefabs.GetEntry(type).Value;
            simpleObjectPools.Add(type,
                 new ObjectPool<GameObject>(() =>
                 {
                     return Instantiate(obj, transform);
                 }, item =>
                 {
                     item.SetActive(true);
                 }, item =>
                 {
                     item.SetActive(false);
                 }, item =>
                 {
                     Destroy(item);
                 }, true, 100));
            return simpleObjectPools[type];
        }
    }

    public GameObject GetSimpleObject(SimpleObjectType type)
    {
        return GetSimpleObjectPool(type).Get();
    }

    public void ReleaseSimpleObject(SimpleObjectType type, GameObject obj)
    {
        GetSimpleObjectPool(type).Release(obj);
    }
}
