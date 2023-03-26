using UnityEngine;

public class MeasuredPhysicsEngine : PhysicsEngine
{
    [SerializeField] private NumStore massStore;
    [SerializeField] private V3Store velocityStore;
    [SerializeField] private V3Store accelerationStore;

    [SerializeField] private NumStore altitudeStore;
    [SerializeField] private NumStore relativeSurfaceAngleStore;

    RaycastHit hit;
    Ray ray;

    private new void Update()
    {
        base.Update();
        if (massStore)
            massStore.SetValue(Mass);
        if (velocityStore)
            velocityStore.SetValue(VelocityVector);
        if (accelerationStore)
            accelerationStore.SetValue(accelerationVector);

        ray = new Ray(transform.position, -gravityTargetOffset);
        Physics.Raycast(ray, out hit, Mathf.Infinity);
        if (altitudeStore)
        {
            altitudeStore.SetValue(Vector3.Distance(transform.position, gravityTarget.position) - gravityTarget.localScale.y / 2);
        }
        if (relativeSurfaceAngleStore)
        {
            relativeSurfaceAngleStore.SetValue(Vector3.Angle(transform.up, hit.point));
        }
    }
}
