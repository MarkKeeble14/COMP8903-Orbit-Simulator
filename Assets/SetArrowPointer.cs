using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetArrowPointer : MonoBehaviour
{
    [SerializeField] private ArrowPointer pointer;
    [SerializeField] private Transform pointAt;
    [SerializeField] private Material material;

    [SerializeField] private Animator anim;
    [SerializeField] private MeasuredPhysicsEngine engine;
    [SerializeField] private float altitudeLimit;

    private void Awake()
    {
        pointer.SetPointAt(transform, pointAt, material);
    }

    private void Update()
    {
        anim.SetBool("Visible", engine.GetDistanceToTarget() > altitudeLimit);

    }
}
