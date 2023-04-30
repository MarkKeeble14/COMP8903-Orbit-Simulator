using UnityEngine;

[CreateAssetMenu(fileName = "Speaker", menuName = "Speaker", order = 0)]
public class Speaker : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    public Color Color;
}
