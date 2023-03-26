using UnityEngine;
using System.Collections;

public abstract partial class RocketEngine : MonoBehaviour
{
    [SerializeField] protected float fuelCapacity;
    protected float fullFuelCapacity;
    [SerializeField] protected float maxThrust;

    [Range(0, 1)]
    [SerializeField] protected float thrustPercent;

    private float currentThrust; // N
    private PhysicsEngine physicsEngine;

    [SerializeField] private bool logOutOfFuel;
    private float kilogramToGramConversion = 1000f;

    protected float pitch;
    protected float yaw;
    protected float roll;

    private void Awake()
    {
        physicsEngine = GetComponent<PhysicsEngine>();
        fullFuelCapacity = fuelCapacity;
    }

    protected void Start()
    {
        physicsEngine.Mass += fuelCapacity;
    }

    void FixedUpdate()
    {
        if (fuelCapacity > FuelThisUpdate())
        {
            physicsEngine.OverrideGroundedCheck = true;
            fuelCapacity -= FuelThisUpdate();
            physicsEngine.Mass -= FuelThisUpdate();

            ExertForce();
        }
        else
        {
            physicsEngine.OverrideGroundedCheck = true;
            if (logOutOfFuel)
                Debug.LogWarning("Out of Rocket Fuel");
        }
    }

    protected void Update()
    {
        // Adjust Rotation
        transform.rotation = Quaternion.Euler(new Vector3(roll, pitch, yaw));
    }

    public void AdjustRotation(RotType type, float v)
    {
        switch (type)
        {
            case RotType.PITCH:
                pitch += v;
                break;
            case RotType.YAW:
                yaw += v;
                break;
            case RotType.ROLL:
                roll += v;
                break;
        }
    }

    float FuelThisUpdate()
    {
        float exhastMassFlow;
        float effectiveExhastVelocity = 4462f; //[m s^-1] 
        exhastMassFlow = currentThrust / effectiveExhastVelocity;

        return exhastMassFlow * Time.deltaTime; //kg
    }

    void ExertForce()
    {
        currentThrust = thrustPercent * maxThrust * kilogramToGramConversion;
        Vector3 thrustVector = transform.up * currentThrust; // N
        physicsEngine.AddForce(thrustVector);
    }

    public void AdjustThrustPercent(float v)
    {
        thrustPercent += v;
        if (thrustPercent > 1) thrustPercent = 1;
        if (thrustPercent < 0) thrustPercent = 0;
    }

    public void Refuel(float amount)
    {
        fuelCapacity += amount;
        if (fuelCapacity > fullFuelCapacity) fuelCapacity = fullFuelCapacity;
    }

    public void FullyRefuel()
    {
        fuelCapacity = fullFuelCapacity;
    }
}

