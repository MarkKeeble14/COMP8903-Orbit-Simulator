using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer), typeof(Rigidbody))]
public abstract class PhysicsEngine : MonoBehaviour
{
    public float Mass // [kg]
    {
        get
        {
            return rb.mass;
        }
        set
        {
            rb.mass = value;
        }
    }
    public Vector3 VelocityVector => rb.velocity; // [m s^-1]
    private Vector3 lastVelocity;
    protected Vector3 accelerationVector;

    private Rigidbody rb;

    [SerializeField] protected Transform gravityTarget;
    protected Vector3 gravityTargetOffset;
    [SerializeField] private float gravity = 9.81f;

    [SerializeField] private bool autoOrient;
    [SerializeField] private float autoOrientSpeed = 1f;

    // Trails
    [SerializeField] private bool drawTrails;
    private LineRenderer lineRenderer;
    [SerializeField] private float trailOffsetScale = .1f;

    private bool grounded;
    public bool OverrideGroundedCheck;
    [SerializeField] private float groundedCheckDistance = 1.25f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        SetupThrustTrails();
    }

    void FixedUpdate()
    {
        ProcessGravity();
        RenderTrails();
    }

    protected void Update()
    {
        // Checks to see if the object is grounded
        Ray ray = new Ray(transform.position, -gravityTargetOffset);
        grounded = Physics.Raycast(ray, groundedCheckDistance);
        if (OverrideGroundedCheck)
        {
            // rb.isKinematic = false;
        }
        else
        {
            // rb.isKinematic = grounded;
        }
    }

    void ProcessGravity()
    {
        // Find Acceleration Vector
        accelerationVector = (rb.velocity - lastVelocity) / Mass;

        // Find Direction of Gravity
        gravityTargetOffset = transform.position - gravityTarget.position;
        rb.AddForce(-gravityTargetOffset.normalized * gravity * rb.mass);

        // Update Last Velocity
        lastVelocity = rb.velocity;

        if (autoOrient) AutoOrient();
    }

    private void AutoOrient()
    {
        Quaternion orientRotation = Quaternion.FromToRotation(-transform.up, gravityTargetOffset) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, orientRotation, Time.deltaTime * autoOrientSpeed);
    }

    // Use this for initialization
    void SetupThrustTrails()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
    }

    public void AddForce(Vector3 force)
    {
        rb.AddForce(force);
    }

    void RenderTrails()
    {
        if (drawTrails)
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position - (rb.velocity * trailOffsetScale));
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }
}