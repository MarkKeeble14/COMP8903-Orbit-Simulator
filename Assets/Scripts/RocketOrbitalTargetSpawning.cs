using UnityEngine;

public class RocketOrbitalTargetSpawning : MonoBehaviour
{

    [SerializeField] private LayerMask rocketLayer;
    [SerializeField] private LayerMask orbitalTargetLayer;

    private GameManager cachedGameManager;
    private GameObject[] targets;
    private int hitCount;

    private void Start()
    {
        cachedGameManager = GameManager._Instance;
    }

    public void SpawnNewOrbitalTarget()
    {
        targets = cachedGameManager.SetNewOrbitalTarget();
        hitCount = 0;
        // Debug.Log("Spawned New Orbital Target, Reset HitCount: " + hitCount);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Haven't set yet or have already maxed out (achieved orbit)
        if (targets == null || targets.Length <= 0 || hitCount > targets.Length - 1)
        {
            return;
        }

        // Debug.Log("HitCount: " + hitCount + ", Target: " + targets[hitCount]);
        // Check if other object is the correct orbital target
        if (other.gameObject != targets[hitCount])
        {
            // Debug.Log("Didn't Hit Correct Target - Hit: " + other.gameObject + ", not " + targets[hitCount]);
            return;
        }
        else
        {
            hitCount++;
            // Debug.Log("Hit Orbital Target: " + other.gameObject + ", HitCount: " + hitCount);
        }

        if (hitCount >= targets.Length)
        {
            // Achieved Orbit
            // Debug.Log("Achieved Orbit");
            cachedGameManager.AchievedOrbit = true;
        }
    }
}
