using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RocketBlowUp : MonoBehaviour
{
    public static RocketBlowUp _Instance { get; private set; }

    private bool armed;

    [SerializeField] private int numOnBlowParticlesIterations = 1;
    [SerializeField] private Vector3 onBlowParticlesSpawnOffset;
    [SerializeField] private GameObject[] onBlowParticles;
    [SerializeField] private float realtimeDelayToOpenOnBlowUI;

    [SerializeField] private DeParentGameObjectData[] deParentOnArm;

    [SerializeField] private GameObjectStateData[] onBlowStateChanges;
    [SerializeField] private AnimatorAnimationTriggerData[] onBlowTriggerAnimations;
    [SerializeField] private GameObjectStateData[] afterDelayStateChanges;

    [SerializeField] private float velocityMagnitudeToArmAt = 25.0f;
    [SerializeField] private float maxVelocityCanCollideAt = 10.0f;
    [SerializeField] private V3Store rocketVelocity;
    [SerializeField] private NumStore rocketAltitude;
    [SerializeField] private float minRocketAltitudeToWarn = 100.0f;
    [SerializeField] private GameObject rocketDangerWarningDisplay;

    [SerializeField] private MonoBehaviour dummy;

    private float gracePeriod = 1.0f;
    private bool canArm;

    [Header("Audio")]
    [SerializeField] private TemporaryAudioSource tempAudioSource;
    [SerializeField] private SimpleAudioClipContainer onBlow;
    [SerializeField] private SimpleAudioClipContainer onArmed;
    [SerializeField] private SimpleAudioClipContainer onCollide;

    public bool AllowArm { get; set; }

    private float lastAltitude;

    private void OnCollisionEnter(Collision collision)
    {
        if (!gameObject.activeInHierarchy) return;
        if (!canArm) return;
        onCollide.PlayOneShot();
        TryBlow();
    }

    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(gameObject);
        }
        _Instance = this;
    }

    private void Start()
    {
        StartCoroutine(GracePeriod());
    }

    public void TryBlow()
    {
        if (armed && GameManager._Instance.RocketVelocityTooHighForImpact)
        {
            BlowUp();
        }
    }

    private IEnumerator GracePeriod()
    {
        yield return new WaitForSeconds(gracePeriod);

        canArm = true;
    }

    private void Update()
    {
        // Determine if rocket velocity is at a dangerous point
        bool rocketVelocityTooHigh = rocketVelocity.GetValue().magnitude > maxVelocityCanCollideAt;
        // Notify Game Manager of rocket velocity so that in the event of a land objective, the game manager knows if the "landing" was actually an fiery explosion
        GameManager._Instance.RocketVelocityTooHighForImpact = rocketVelocityTooHigh;
        // Enable/disable warning display depending on if velocity is too high, we are close enough to the planet to care, and we are moving towards the planet (i.e., falling)
        rocketDangerWarningDisplay.SetActive(armed && rocketVelocityTooHigh && rocketAltitude.GetValue() <= minRocketAltitudeToWarn && lastAltitude > rocketAltitude.GetValue());
        lastAltitude = rocketAltitude.GetValue();

        if (!canArm) return;
        if (!AllowArm) return;
        if (rocketVelocity.GetValue().magnitude > velocityMagnitudeToArmAt)
        {
            if (armed) return;
            Arm();
        }
    }

    public void Arm()
    {
        armed = true;
        GameManager._Instance.RocketArmed = true;

        onArmed.PlayOneShot();

        foreach (DeParentGameObjectData data in deParentOnArm)
        {
            if (data.DisableCollider)
            {
                data.Transform.GetComponent<Collider>().enabled = false;
            }

            data.Transform.SetParent(null);

            if (data.AddRigidBody)
            {
                Rigidbody rb = data.Transform.gameObject.AddComponent<Rigidbody>();

                rb.AddForce(
                    RandomHelper.RandomFloat(-data.AddedForcePerAxis.x, data.AddedForcePerAxis.x),
                    RandomHelper.RandomFloat(-data.AddedForcePerAxis.y, data.AddedForcePerAxis.y),
                    RandomHelper.RandomFloat(-data.AddedForcePerAxis.z, data.AddedForcePerAxis.z));

                rb.AddTorque(
                    RandomHelper.RandomFloat(-data.AddedTorquePerAxis.x, data.AddedTorquePerAxis.x),
                    RandomHelper.RandomFloat(-data.AddedTorquePerAxis.y, data.AddedTorquePerAxis.y),
                    RandomHelper.RandomFloat(-data.AddedTorquePerAxis.z, data.AddedTorquePerAxis.z));
            }
        }
    }

    private void BlowUp()
    {
        // Notify Game Manager
        GameManager._Instance.RocketExploded = true;

        // Lock the Time Scale to 1
        UIManager._Instance.LockTimeScale(1);

        // Spawn a dummy object to run the coroutine before destroying the object
        Instantiate(dummy).StartCoroutine(OpenOnBlowUI());

        for (int i = 0; i < numOnBlowParticlesIterations; i++)
        {
            foreach (GameObject particle in onBlowParticles)
            {
                Vector3 spawnPos = transform.position
                    + new Vector3(
                        RandomHelper.RandomFloat(-onBlowParticlesSpawnOffset.x, onBlowParticlesSpawnOffset.x),
                        RandomHelper.RandomFloat(-onBlowParticlesSpawnOffset.y, onBlowParticlesSpawnOffset.y),
                        RandomHelper.RandomFloat(-onBlowParticlesSpawnOffset.z, onBlowParticlesSpawnOffset.z));
                Instantiate(particle, spawnPos, Quaternion.identity);
                // Audio
                Instantiate(tempAudioSource, spawnPos, Quaternion.identity).Play(onBlow);
            }
        }

        foreach (GameObjectStateData data in onBlowStateChanges)
        {
            data.GameObject.SetActive(data.State == GameObjectState.ENABLED ? true : false);
        }

        foreach (AnimatorAnimationTriggerData data in onBlowTriggerAnimations)
        {
            data.Animator.SetTrigger(data.TriggerParameter);
        }

        //
        gameObject.SetActive(false);
    }

    private IEnumerator OpenOnBlowUI()
    {
        yield return new WaitForSecondsRealtime(realtimeDelayToOpenOnBlowUI);

        foreach (GameObjectStateData data in afterDelayStateChanges)
        {
            data.GameObject.SetActive(data.State == GameObjectState.ENABLED ? true : false);
        }
    }
}
