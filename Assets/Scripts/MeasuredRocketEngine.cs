using UnityEngine;

[RequireComponent(typeof(PhysicsEngine))]
public class MeasuredRocketEngine : RocketEngine
{
    [SerializeField] private NumStore thrustPercentStore;
    [SerializeField] private NumStore fuelCapacityStore;
    [SerializeField] private NumStore maxThrustStore;

    [SerializeField] private NumStore pitchStore;
    [SerializeField] private NumStore yawStore;
    [SerializeField] private NumStore rollStore;


    protected override void SetFuelCapacityAndFill(float f)
    {
        base.SetFuelCapacityAndFill(f);
    }

    public override void SetInitialFuelCapacity(float f, bool autoFill)
    {
        fuelCapacityStore.SetMaxValue(f);
        base.SetInitialFuelCapacity(f, autoFill);
    }

    protected override void SetFuelCapacity(float f)
    {
        fuelCapacityStore.SetValue(f);
        base.SetFuelCapacity(f);
    }

    public override void SetMaximumThrust(float f)
    {
        maxThrustStore.SetValue(f);
        base.SetMaximumThrust(f);
    }

    protected new void Update()
    {
        thrustPercentStore.SetValue(thrustPercent);

        rollStore.SetValue(roll);
        pitchStore.SetValue(pitch);
        yawStore.SetValue(yaw);

        base.Update();
    }
}

