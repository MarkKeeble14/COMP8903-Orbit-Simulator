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

    float rotPerpendicular;
    float rotParallel;

    private float seed;
    [SerializeField] float frequency = 25;
    [SerializeField] Vector3 maximumTranslationShake = Vector3.one * 0.5f;
    Vector3 shake;

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
            maximumTranslationShake.x * (Mathf.PerlinNoise(seed, Time.time * frequency) * 2 - 1),
            maximumTranslationShake.y * (Mathf.PerlinNoise(seed + 1, Time.time * frequency) * 2 - 1),
            maximumTranslationShake.z * (Mathf.PerlinNoise(seed + 2, Time.time * frequency) * 2 - 1)
        ) * 0.5f;

        switch (mode)
        {
            case CameraMode.FIXED:
                lookRotation = Quaternion.Euler(new Vector2(rotParallel, rotPerpendicular));
                lookDirection = lookRotation * Vector3.forward;
                lookPosition = followedObject.position - lookDirection * distance;
                transform.SetPositionAndRotation(lookPosition + shake, lookRotation);

                break;
            case CameraMode.ORBITAL:
                lookRotation = Quaternion.Euler(new Vector3(rotParallel, rotPerpendicular,
                    Vector3.Angle(Vector3.up, followedObject.gameObject.GetComponent<Rigidbody>().velocity)));
                lookDirection = lookRotation * Vector3.forward;
                lookPosition = followedObject.position - lookDirection * distance;
                transform.SetPositionAndRotation(lookPosition, lookRotation);
                break;
        }

        /*
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (mode == CameraMode.FIXED)
            {
                mode = CameraMode.ORBITAL;
            }
            else
            {
                mode = CameraMode.FIXED;
            }
        }
        */

        // Rotation
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rotParallel += 30.0f * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            rotParallel -= 30.0f * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rotPerpendicular += 30.0f * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            rotPerpendicular -= 30.0f * Time.deltaTime;
        }
    }
}
