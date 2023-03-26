using UnityEngine;

public abstract class StoreDisplay : MonoBehaviour
{
    [SerializeField] private string label;
    [SerializeField] private string seperator = ": ";
    public string LabelContent => label + seperator;
    public virtual string StringContent => LabelContent;
}
