using UnityEngine;

public abstract class StoreDisplay : MonoBehaviour
{
    [SerializeField] private string label;
    [SerializeField] private string seperator = ": ";
    [SerializeField] private string suffix;
    public string LabelContent => label + seperator;
    public virtual string StringContent => LabelContent + Suffix;
    public virtual string Suffix => suffix;
}
