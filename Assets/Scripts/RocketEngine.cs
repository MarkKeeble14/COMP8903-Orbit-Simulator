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

    [SerializeField] private bool useObjectPooler;

    [SerializeField] private Flame flamePrefab;
    [SerializeField] private Smoke smokePrefab;
    [SerializeField] private Transform enginePosition;
    [SerializeField] private Vector2 minMaxFlamesVisualsMultiplier = new Vector2(1, 5);

    [Header("Smoke Stats")]
    [SerializeField] private int numSmokePerFlame = 3;
    [SerializeField] private float flameSpeed;

    [Header("Audio")]
    [SerializeField] private ContinuousAudioSource thrustersSFX;

    [Header("References")]
    [SerializeField] private RocketCameraController[] rocketCameraControllers;
    [SerializeField] private RocketOrbitalTargetSpawning rocketOrbitalTargetSpawning;
    private float addedFuelCapacity;

    private void Awake()
    {
        physicsEngine = GetComponent<PhysicsEngine>();
    }

    protected virtual void SetFuelCapacityAndFill(float f)
    {
        SetInitialFuelCapacity(f, true);
    }

    public virtual void SetInitialFuelCapacity(float f, bool autoFill)
    {
        physicsEngine.Mass -= addedFuelCapacity;
        fullFuelCapacity = f;
        addedFuelCapacity = f;
        if (autoFill)
            SetFuelCapacity(f);
        physicsEngine.Mass += addedFuelCapacity;
    }

    public virtual void SetMaximumThrust(float f)
    {
        maxThrust = f;
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
            Flame flame;
            if (useObjectPooler)
            {
                flame = ObjectPooler._FlamePool.Get();
                flame.transform.position = enginePosition.position;
            }
            else
            {
                flame = Instantiate(flamePrefab, enginePosition.position, Quaternion.identity);
            }

            // Add force to flame
            Vector3 force = -transform.up * flameSpeed;
            flame.Set(physicsEngine.VelocityVector, force, physicsEngine, physicsEngine.GetGravityTarget(), useObjectPooler);
            SpawnSmoke(physicsEngine.VelocityVector, force, numSmokePerFlame);
        }
    }

    private void SpawnSmoke(Vector3 velocity, Vector3 force, int numToSpawn)
    {
        for (int i = 0; i < numToSpawn; i++)
        {
            Smoke spawned;
            if (useObjectPooler)
            {
                spawned = ObjectPooler._SmokePool.Get();
                spawned.transform.position = enginePosition.position;
            }
            else
            {
                spawned = Instantiate(smokePrefab, enginePosition.position, Quaternion.identity);
            }
            spawned.Set(velocity, force, physicsEngine.GetGravityTarget(), useObjectPooler);
        }
    }

    void FixedUpdate()
    {
        if (!Active) return;

        if (fuelCapacity > FuelThisUpdate())
        {
            float fuelThisUpdate = FuelThisUpdate();
            SetFuelCapacity(fuelCapacity - fuelThisUpdate);
            physicsEngine.Mass -= fuelThisUpdate;
            addedFuelCapacity -= fuelThisUpdate;

            ExertForce();

            rocketOrbitalTargetSpawning.SpawnNewOrbitalTarget();
        }
        else
        {
            fuelCapacity = 0;
            if (logOutOfFuel)
            {
                Debug.LogWarning("Out of Rocket Fuel");
            }
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

    public void ParseMaxThrust(string s)
    {
        float maxThrust;
        if (Utils.ParseFloat(s, out maxThrust))
        {
            SetMaximumThrust(maxThrust);
        };
    }

    public void ParseFuelCapacity(string s)
    {
        float fuelCapacity;
        if (Utils.ParseFloat(s, out fuelCapacity))
        {
            SetFuelCapacityAndFill(fuelCapacity);
        };
    }

    protected virtual void SetFuelCapacity(float f)
    {
        fuelCapacity = f;
    }
}

