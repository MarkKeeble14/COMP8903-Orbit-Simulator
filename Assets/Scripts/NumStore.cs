using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NumStore", menuName = "NumStore", order = 0)]
public class NumStore : GameStore
{
    [SerializeField] private float defaultValue;
    private float value;

    [SerializeField] private bool shouldRound;
    [SerializeField] private int roundTo = 1;

    [SerializeField] private bool hasMaxValue;
    public bool HasMaxValue => hasMaxValue;
    [SerializeField] private float maxValue;
    public float MaxValue => maxValue;
    [SerializeField] private bool printValueWhenSet;

    public float GetValue()
    {
        return value;
    }

    public void SetValue(float v)
    {
        if (hasMaxValue)
        {
            if (v > maxValue)
                v = maxValue;
        }

        if (shouldRound)
            value = (float)System.Math.Round(v, roundTo);
        else
            value = v;

        if (printValueWhenSet)
            Debug.Log(name + ": " + value);
    }

    public void SetMaxValue(float v)
    {
        hasMaxValue = true;
        maxValue = v;
    }

    public void RemoveMaxValue()
    {
        hasMaxValue = false;
    }

    public override void Reset()
    {
        value = defaultValue;
    }
}