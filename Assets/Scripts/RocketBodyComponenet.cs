using UnityEngine;

public class RocketBodyComponenet : MonoBehaviour
{
    private RocketBlowUp rocketBlowUp;

    private void Awake()
    {
        rocketBlowUp = GetComponentInParent<RocketBlowUp>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        rocketBlowUp.TryBlow();
    }
}
