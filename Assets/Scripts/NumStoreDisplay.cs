using UnityEngine;
using TMPro;

public class NumStoreDisplay : StoreDisplay
{
    [SerializeField] private NumStore store;
    [SerializeField] private bool round;
    [SerializeField] private int digits;

    public override string StringContent => LabelContent + (!round ? store.GetValue() : System.Math.Round(store.GetValue(), digits)).ToString() + Suffix;


    private TextMeshProUGUI text;

    private void Awake()
    {
        // 
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        text.text = StringContent;
    }
}
