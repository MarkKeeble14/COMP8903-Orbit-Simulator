using UnityEngine;
using TMPro;

public class ScientificNotationStoreDisplay : StoreDisplay
{
    [SerializeField] private ScientificNotationStore store;

    public override string StringContent => LabelContent + store.GetValue().ToString() + Suffix;


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