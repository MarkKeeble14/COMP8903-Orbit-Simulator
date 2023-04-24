using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketCameraController : MonoBehaviour
{
    public enum CameraMode
    {
        FIXED,
        ORBITAL
    }

    public Transform downObject;

    public Transform followedObject;

    public float distance = 50;

    public CameraMode mode;

    private float rotPerpendicular;
    private float rotParallel;
    private float zoom;
    [SerializeField] private float maxZoom;
    [SerializeField] private float minZoom;
    [SerializeField] private float zoomRate;

    private float seed;
    [SerializeField] float frequency = 25;
    [SerializeField] Vector3 maximumTranslationShake = Vector3.one * 0.5f;
    Vector3 shake;
    public bool ShakeActive;

    private void Awake()
    {
        seed = Random.value;
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion lookRotation;
        Vector3 lookDirection;
        Vector3 lookPosition;


        shake = new Vector3(
            maximumTranslationShake.x * (Mathf.PerlinNoise(seed, Time.unscaledTime * frequency) * 2 - 1),
            maximumTranslationShake.y * (Mathf.PerlinNoise(seed + 1, Time.unscaledTime * frequency) * 2 - 1),
            maximumTranslationShake.z * (Mathf.PerlinNoise(seed + 2, Time.unscaledTime * frequency) * 2 - 1)
        ) * 0.5f;

        switch (mode)
        {
            case CameraMode.FIXED:
                lookRotation = Quaternion.Euler(new Vector2(rotParallel, rotPerpendicular));
                lookDirection = lookRotation * Vector3.forward;
                lookPosition = followedObject.position - lookDirection * (distance + zoom);
                transform.SetPositionAndRotation(lookPosition + (ShakeActive ? shake : Vector3.zero), lookRotation);

                break;
            case CameraMode.ORBITAL:
                lookRotation = Quaternion.Euler(new Vector3(rotParallel, rotPerpendicular,
                    Vector3.Angle(Vector3.up, followedObject.gameObject.GetComponent<Rigidbody>().velocity)));
                lookDirection = lookRotation * Vector3.forward;
                lookPosition = followedObject.position - lookDirection * (distance + zoom);
                transform.SetPositionAndRotation(lookPosition + (ShakeActive ? shake : Vector3.zero), lookRotation);
                break;
        }

        if (Input.GetKey(KeyCode.Comma))
        {
            zoom -= zoomRate * Time.unscaledDeltaTime;
            if (zoom < minZoom)
                zoom = minZoom;
        }
        else if (Input.GetKey(KeyCode.Period))
        {
            zoom += zoomRate * Time.unscaledDeltaTime;
            if (zoom > maxZoom)
                zoom = maxZoom;
        }

        // Rotation
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rotParallel += 30.0f * Time.unscaledDeltaTime;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            rotParallel -= 30.0f * Time.unscaledDeltaTime;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rotPerpendicular += 30.0f * Time.unscaledDeltaTime;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            rotPerpendicular -= 30.0f * Time.unscaledDeltaTime;
        }
    }
}
