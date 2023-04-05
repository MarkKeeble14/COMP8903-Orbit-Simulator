using UnityEngine;

[CreateAssetMenu(fileName = "ScientificNotationStore", menuName = "ScientificNotationStore", order = 0)]
public class ScientificNotationStore : GameStore
{
    [SerializeField] private ScientificNotation defaultValue;
    private ScientificNotation value;

    public ScientificNotation GetValue()
    {
        return value;
    }

    public void SetValue(ScientificNotation v)
    {
        value = v;
    }

    public void SetNumber(float v)
    {
        value.number = v;
    }

    public void SetExpononent(int v)
    {
        value.exponent = v;
    }

    public override void Reset()
    {
        Debug.Log("SN Reset");
        value = defaultValue;
    }
}
