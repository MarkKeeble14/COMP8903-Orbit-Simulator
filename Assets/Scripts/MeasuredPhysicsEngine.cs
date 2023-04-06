using UnityEngine;

public class MeasuredPhysicsEngine : PhysicsEngine
{
    [SerializeField] private NumStore massStore;
    [SerializeField] private V3Store velocityStore;
    [SerializeField] private V3Store accelerationStore;

    [SerializeField] private NumStore altitudeStore;
    [SerializeField] private NumStore relativeSurfaceAngleStore;

    [SerializeField] private LayerMask gravityTargetLayer;

    RaycastHit hit;
    Ray ray;

    public float GetDistanceToTarget()
    {
        return altitudeStore.GetValue();
    }

    private void Update()
    {
        if (massStore)
            massStore.SetValue(Mass);
        if (velocityStore)
            velocityStore.SetValue(VelocityVector);
        if (accelerationStore)
            accelerationStore.SetValue(accelerationVector);

        if (gravityTarget)
        {
            if (altitudeStore)
            {
                altitudeStore.SetValue(Vector3.Distance(transform.position, gravityTarget.position) - gravityTarget.localScale.y / 2);
            }

            if (relativeSurfaceAngleStore)
            {
                ray = new Ray(transform.position, (gravityTarget.position - transform.position).normalized);
                Physics.Raycast(ray, out hit, Mathf.Infinity, gravityTargetLayer);
                Debug.DrawRay(ray.origin, ray.direction, Color.red, 5f);
                if (hit.transform != null)
                {
                    relativeSurfaceAngleStore.SetValue(Vector3.Angle(transform.up, hit.point));
                }
            }
        }
    }
}
