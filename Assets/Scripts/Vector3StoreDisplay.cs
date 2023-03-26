using UnityEngine;
using TMPro;

public class Vector3StoreDisplay : StoreDisplay
{
    [SerializeField] private V3Store store;
    public override string StringContent => LabelContent + Utils.ConvVector3ToString(store.GetValue());

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
