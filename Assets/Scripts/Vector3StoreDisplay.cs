using UnityEngine;
using TMPro;
using System;

public class Vector3StoreDisplay : StoreDisplay
{
    [SerializeField] private V3Store store;

    [SerializeField] private bool useMagnitude;
    [SerializeField] private float minValueToShow;
    [SerializeField] private bool applySuffixPerComponent;

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
                    "{0:" + maxDigitLengthString + ".00}" + (applySuffixPerComponent ? Suffix : "") + ", " +
                    "{1:" + maxDigitLengthString + ".00}" + (applySuffixPerComponent ? Suffix : "") + ", " +
                    "{2:" + maxDigitLengthString + ".00}" + (applySuffixPerComponent ? Suffix : "") +
                    ">";
                return LabelContent + Utils.ConvVector3ToStringAbs(store.GetValue(), 1,
                    format, minValueToShow) + (!applySuffixPerComponent ? Suffix : "");
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
