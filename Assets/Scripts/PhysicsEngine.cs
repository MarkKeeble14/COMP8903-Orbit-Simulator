using System;
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

    public ScientificNotationStore planetMass;
    public ScientificNotationStore rocketMass;
    public ScientificNotationStore gConst;

    public Vector3 VelocityVector => rb.velocity; // [m s^-1]
    private Vector3 lastVelocity;
    protected Vector3 accelerationVector;

    private Rigidbody rb;

    [SerializeField] protected Transform gravityTarget;
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

    public void SetVelocity(Vector3 velocity)
    {
        rb.velocity = velocity;
    }

    void ProcessGravity()
    {
        //Find Local Gravity
        Vector3 gSource = gravityTarget.position - transform.position;
        float gravForce = (float)(gConst.GetValue() * ((planetMass.GetValue() * new ScientificNotation(Mass, 0)) / new ScientificNotation(gSource.magnitude * gSource.magnitude, 0)));

        // Find Direction of Gravity
        // Debug.Log("a: " + gravForce / (float)rocketMass);
        rb.AddForce(gSource.normalized * gravForce);

        // Find Acceleration Vector
        accelerationVector = (rb.velocity - lastVelocity) / Time.fixedDeltaTime;
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

    public Transform GetGravityTarget()
    {
        return gravityTarget;
    }

    public void SetMass(string s)
    {
        float mass;
        if (Utils.ParseFloat(s, out mass))
        {
            Mass = mass;
        };
    }

    public void SetPlanetMassNumber(string s)
    {
        float massNumber;
        if (Utils.ParseFloat(s, out massNumber))
        {
            planetMass.SetNumber(massNumber);
        };
    }

    public void SetPlanetMassExponent(string s)
    {
        int massExponent;
        if (Utils.ParseInt(s, out massExponent))
        {
            planetMass.SetExpononent(massExponent);
        };
    }
}