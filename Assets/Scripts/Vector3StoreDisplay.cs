using UnityEngine;
using TMPro;
using System;

public class Vector3StoreDisplay : StoreDisplay
{
    [SerializeField] private V3Store store;

    [SerializeField] private bool useMagnitude;
    [SerializeField] private float minValueToShow;

    public override string StringContent
    {
        get
        {
            string maxDigitLengthString = Utils.GetRepeatingString("0", Utils.GetMaxDigits(store.GetValue()));
            if (useMagnitude)
            {
                float magnitude = store.GetValue().magnitude;
                return LabelContent + (magnitude > minValueToShow ? magnitude : 0) + Suffix;
            }
            else
            {
                string format = "<" +
                    "{0:" + maxDigitLengthString + ".00}, " +
                    "{1:" + maxDigitLengthString + ".00}, " +
                    "{2:" + maxDigitLengthString + ".00}" +
                    ">";
                return LabelContent + Utils.ConvVector3ToStringAbs(store.GetValue(), 1,
                    format, minValueToShow) + Suffix;
            }
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
