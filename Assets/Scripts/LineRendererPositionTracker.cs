using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererPositionTracker : MonoBehaviour
{
    [SerializeField] private int maxSamples;
    [SerializeField] private float timeBetweenSamples;
    private float sampleTimer;
    [SerializeField] private LineRenderer lr;
    [SerializeField] private float widthMultiplier;

    // Update is called once per frame
    void Update()
    {
        lr.widthMultiplier = widthMultiplier;

        if (sampleTimer <= 0)
        {
            lr.positionCount++;

            if (lr.positionCount > maxSamples)
            {
                for (int i = 1; i < lr.positionCount - 1; i++)
                {
                    lr.SetPosition(i - 1, lr.GetPosition(i));
                }
            }

            lr.SetPosition(lr.positionCount - 1, transform.position);
            sampleTimer = timeBetweenSamples;
        }
        else
        {
            sampleTimer -= Time.deltaTime;
        }
    }

    public void ClearPositions()
    {
        lr.positionCount = 0;
    }
}
