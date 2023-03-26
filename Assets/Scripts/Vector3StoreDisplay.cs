using UnityEngine;
using TMPro;
using System;

public class Vector3StoreDisplay : StoreDisplay
{
    [SerializeField] private V3Store store;
    public override string StringContent
    {
        get
        {
            string maxDigitLengthString = Utils.GetRepeatingString("0", Utils.GetMaxDigits(store.GetValue()));
            string format = "<" +
                "{0:" + maxDigitLengthString + ".00}, " +
                "{1:" + maxDigitLengthString + ".00}, " +
                "{2:" + maxDigitLengthString + ".00}" +
                ">";
            return LabelContent + Utils.ConvVector3ToString(store.GetValue(), 1,
                format) + Suffix;
        }
    }

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
