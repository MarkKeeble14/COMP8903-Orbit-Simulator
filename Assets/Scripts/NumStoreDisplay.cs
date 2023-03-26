using UnityEngine;
using TMPro;

public class NumStoreDisplay : StoreDisplay
{
    [SerializeField] private NumStore store;
    public override string StringContent => LabelContent + store.GetValue().ToString();

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
