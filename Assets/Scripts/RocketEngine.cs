using UnityEngine;

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

    [SerializeField] private bool active;
    public bool Active { get { return active; } set { active = value; } }

    [SerializeField] private Flame flamePrefab;
    [SerializeField] private Smoke smokePrefab;
    [SerializeField] private Transform enginePosition;
    [SerializeField] private Vector2 minMaxFlamesVisualsMultiplier = new Vector2(1, 5);

    [Header("Smoke Stats")]
    [SerializeField] private int numSmokePerFlame = 3;
    [SerializeField] private float flameSpeed;

    [Header("Audio")]
    [SerializeField] private ContinuousAudioSource thrustersSFX;

    [SerializeField] private RocketCameraController[] rocketCameraControllers;

    private void Awake()
    {
        physicsEngine = GetComponent<PhysicsEngine>();
        fullFuelCapacity = fuelCapacity;
    }

    protected void Start()
    {
        physicsEngine.Mass += fuelCapacity;
    }

    protected void Update()
    {
        bool thrustersAreFiring = thrustPercent > 0 && Active && fuelCapacity > 0;
        thrustersSFX.Active = thrustersAreFiring;

        foreach (RocketCameraController cameraController in rocketCameraControllers)
        {
            cameraController.ShakeActive = thrustersAreFiring;
        }


        if (!thrustersAreFiring) return;

        int mult = Mathf.RoundToInt(Mathf.Lerp(minMaxFlamesVisualsMultiplier.x, minMaxFlamesVisualsMultiplier.y, thrustPercent));
        for (int i = 0; i < mult; i++)
        {
            // Spawn Flame
            Flame flame = Instantiate(flamePrefab, enginePosition.position, Quaternion.identity);

            // Add force to flame
            Vector3 force = -transform.up * flameSpeed;
            flame.Set(physicsEngine.VelocityVector, force, physicsEngine, physicsEngine.GetGravityTarget());
            SpawnSmoke(physicsEngine.VelocityVector, force, numSmokePerFlame);
        }
    }

    private void SpawnSmoke(Vector3 velocity, Vector3 force, int numToSpawn)
    {
        for (int i = 0; i < numToSpawn; i++)
        {
            Smoke spawned = Instantiate(smokePrefab, enginePosition.position, Quaternion.identity);
            spawned.Set(velocity, force, physicsEngine.GetGravityTarget());
        }
    }

    void FixedUpdate()
    {
        if (!Active) return;

        if (fuelCapacity > FuelThisUpdate())
        {
            fuelCapacity -= FuelThisUpdate();
            physicsEngine.Mass -= FuelThisUpdate();

            ExertForce();
        }
        else
        {
            fuelCapacity = 0;
            if (logOutOfFuel)
                Debug.LogWarning("Out of Rocket Fuel");
        }
    }

    public void AdjustRotation(RotType type, float v)
    {
        switch (type)
        {
            case RotType.PITCH:
                pitch += v;
                transform.Rotate(Vector3.right, v);
                break;
            case RotType.YAW:
                yaw += v;
                transform.Rotate(Vector3.forward, v);
                break;
            case RotType.ROLL:
                roll += v;
                transform.Rotate(Vector3.up, v);
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
        if (thrustPercent <= 0) return;
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

    public void SetMaxThrust(string s)
    {
        float maxThrust;
        if (Utils.ParseFloat(s, out maxThrust))
        {
            this.maxThrust = maxThrust;
        };
    }

    public void SetFuelCapacity(string s)
    {
        float fuelCapacity;
        if (Utils.ParseFloat(s, out fuelCapacity))
        {
            SetFuelCapacity(fuelCapacity);
        };
    }

    protected virtual void SetFuelCapacity(float f)
    {
        fuelCapacity = f;
        fullFuelCapacity = f;
    }
}

