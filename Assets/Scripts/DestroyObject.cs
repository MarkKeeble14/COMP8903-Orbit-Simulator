using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    [SerializeField] private CanvasGroup group;

    private bool armed;

    private void Awake()
    {
        group.alpha = 0;
    }

    private void Update()
    {
        if (group.alpha > 0)
        {
            armed = true;
        }
        if (group.alpha == 0 && armed)
        {
            Destroy(gameObject);
        }
    }
}
