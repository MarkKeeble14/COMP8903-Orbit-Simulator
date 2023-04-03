using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;


[RequireComponent(typeof(LineRenderer), typeof(Rigidbody))]
public class PhysicsEngine : MonoBehaviour
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

    public ScientificNotation planetMass = new ScientificNotation(1.5f, 17);
    public ScientificNotation rocketMass = new ScientificNotation(1.0f, 3);

    public ScientificNotation gConst = new ScientificNotation(6.674f, -11);
    public Vector3 VelocityVector => rb.velocity; // [m s^-1]
    private Vector3 lastVelocity;
    protected Vector3 accelerationVector;

    private Rigidbody rb;

    [SerializeField] protected Transform gravityTarget;
    protected Vector3 gravityTargetOffset;
//    [SerializeField] private float gravity = 9.81f;

    // Trails
    [SerializeField] private bool drawTrails;
    private LineRenderer lineRenderer;
    [SerializeField] private float trailOffsetScale = .1f;

    [SerializeField] private bool useGravity = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        SetupThrustTrails();
    }

    void FixedUpdate()
    {
        RenderTrails();
        if (useGravity)
            ProcessGravity();
    }

    protected void Update()
    {
        // 
    }

    void ProcessGravity()
    {
        //Find Local Gravity
        Vector3 gSource = gravityTarget.position - transform.position;
        float gravForce = (float)(gConst * ((planetMass * new ScientificNotation(Mass, 0)) / new ScientificNotation(gSource.magnitude * gSource.magnitude, 0)));

        // Find Acceleration Vector
        accelerationVector = (rb.velocity - lastVelocity) / Mass;

        // Find Direction of Gravity
        Debug.Log("a: " + gravForce / (float)rocketMass);
        rb.AddForce(gSource.normalized * gravForce);

        // Update Last Velocity
        lastVelocity = rb.velocity;
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

    public void SetMass(string s)
    {
        float mass;
        if (Utils.ParseFloat(s, out mass))
        {
            Mass = mass;
        };
    }
}