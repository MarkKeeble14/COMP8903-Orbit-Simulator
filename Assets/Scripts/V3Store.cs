using UnityEngine;

[CreateAssetMenu(fileName = "Vector3Store", menuName = "Vector3Store", order = 0)]
public class V3Store : GameStore
{
    [SerializeField] private Vector3 defaultValue;
    private Vector3 value;

    public Vector3 GetValue()
    {
        return value;
    }

    public void SetValue(Vector3 v)
    {
        value = v;
    }

    public override void Reset()
    {
        value = defaultValue;
    }
}
