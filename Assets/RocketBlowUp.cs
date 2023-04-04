using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RocketBlowUp : MonoBehaviour
{
    private bool armed;

    [SerializeField] private int numOnBlowParticlesIterations = 1;
    [SerializeField] private Vector3 onBlowParticlesSpawnOffset;
    [SerializeField] private GameObject[] onBlowParticles;
    [SerializeField] private float realtimeDelayToOpenOnBlowUI;

    [SerializeField] private DeParentGameObjectData[] deParentOnArm;

    [SerializeField] private GameObjectStateData[] onBlowStateChanges;
    [SerializeField] private AnimatorAnimationTriggerData[] onBlowTriggerAnimations;
    [SerializeField] private GameObjectStateData[] afterDelayStateChanges;

    [SerializeField] private float velocityMagnitudeToArmAt;
    [SerializeField] private float maxVelocityCanCollideAt = 10.0f;
    [SerializeField] private V3Store rocketVelocity;

    [SerializeField] private MonoBehaviour dummy;

    private float gracePeriod = 1.0f;
    private bool canArm;

    [Header("Audio")]
    [SerializeField] private TemporaryAudioSource tempAudioSource;
    [SerializeField] private AudioClipContainer onBlow;
    [SerializeField] private AudioClipContainer onArmed;
    [SerializeField] private AudioClipContainer onCollide;

    private void OnCollisionEnter(Collision collision)
    {
        if (!gameObject.activeInHierarchy) return;
        if (!canArm) return;
        onCollide.PlayOneShot();
        TryBlow();
    }

    private void Start()
    {
        StartCoroutine(GracePeriod());
    }

    public void TryBlow()
    {
        if (armed && rocketVelocity.GetValue().magnitude > maxVelocityCanCollideAt)
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
        if (!canArm) return;
        if (rocketVelocity.GetValue().magnitude > velocityMagnitudeToArmAt)
        {
            if (armed) return;
            Arm();
        }
    }

    public void Arm()
    {
        armed = true;

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
