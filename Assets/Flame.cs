using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flame : PhysicsEngine
{
    [SerializeField] private float shrinkSpeed;
    [SerializeField] private Vector2 randomSize;
    [SerializeField] private List<Material> possibleMaterial = new List<Material>();
    [SerializeField] private Vector2 minMaxAccelerationMagnitude = new Vector2(0, 25);
    [SerializeField] private Vector2 minMaxResultantSize = new Vector2(1, 5);
    [SerializeField] private Vector3 shiftPosBounds = new Vector3(.5f, .5f, .5f);
    [SerializeField] private new Renderer renderer;

    private void ChangeSize(float changeValue)
    {
        float sizeResult = MathHelper.Normalize(changeValue, minMaxAccelerationMagnitude.x, minMaxAccelerationMagnitude.y,
            minMaxResultantSize.x, minMaxResultantSize.y);
        if (sizeResult > minMaxResultantSize.y) sizeResult = minMaxResultantSize.y;
        transform.localScale = transform.localScale + (Vector3.one * sizeResult);
    }

    public void Set(Vector3 velocity, Vector3 force, PhysicsEngine adder, Transform gravityTarget)
    {
        this.gravityTarget = gravityTarget;

        transform.position += new Vector3(Random.Range(-shiftPosBounds.x, shiftPosBounds.x),
    Random.Range(-shiftPosBounds.y, shiftPosBounds.y),
    Random.Range(-shiftPosBounds.z, shiftPosBounds.z));

        // Set size randomly
        transform.localScale = transform.localScale + (Vector3.one * Random.Range(randomSize.x, randomSize.y));

        // Set material randomly
        renderer.material = possibleMaterial[Random.Range(0, possibleMaterial.Count)];

        SetVelocity(velocity);
        // AddForce(force, adder);
        ChangeSize(force.magnitude);

        StartCoroutine(Lifetime());
    }

    private IEnumerator Lifetime()
    {
        // Scale down over lifetime
        while (Vector3.Distance(transform.localScale, Vector3.zero) > .1f)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, Time.deltaTime * shrinkSpeed);
            yield return null;
        }

        Destroy(gameObject);
    }
}

