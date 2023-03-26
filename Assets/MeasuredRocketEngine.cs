﻿using UnityEngine;

[RequireComponent(typeof(PhysicsEngine))]
public class MeasuredRocketEngine : RocketEngine
{
    [SerializeField] private NumStore fuelCapacityStore;
    [SerializeField] private NumStore maxThrustStore;
    [SerializeField] private NumStore thrustPercentStore;

    [SerializeField] private NumStore pitchStore;
    [SerializeField] private NumStore yawStore;
    [SerializeField] private NumStore rollStore;

    private new void Start()
    {
        fuelCapacityStore.SetMaxValue(fullFuelCapacity);
        base.Start();
    }

    private new void Update()
    {
        maxThrustStore.SetValue(maxThrust);
        thrustPercentStore.SetValue(thrustPercent);
        fuelCapacityStore.SetValue(fuelCapacity);

        rollStore.SetValue(roll);
        pitchStore.SetValue(pitch);
        yawStore.SetValue(yaw);
        base.Update();
    }
}
