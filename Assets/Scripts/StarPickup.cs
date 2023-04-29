using System;
using UnityEngine;

public class StarPickup : MonoBehaviour
{
    public static StarPickup _Instance { get; private set; }
    [SerializeField] private LayerMask rocketLayer;
    private bool canPickup = true;

    [SerializeField] private GameObject[] spawnOnPickup;
    [SerializeField] private Animator anim;
    [SerializeField] private SimpleAudioClipContainer onPickupSFX;
    [SerializeField] private TemporaryAudioSource tempAudioSource;
    [SerializeField] private new Collider collider;

    public bool HasBeenPickedUp { get; private set; }

    private void Awake()
    {
        _Instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canPickup) return;
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, rocketLayer)) return;
        canPickup = false;

        foreach (GameObject obj in spawnOnPickup)
        {
            Instantiate(obj, transform.position, Quaternion.identity);
        }

        HasBeenPickedUp = true;
        anim.SetTrigger("Fade");
        collider.enabled = false;

        // Audio
        Instantiate(tempAudioSource, transform.position, Quaternion.identity).Play(onPickupSFX);
    }

    public void BringBack()
    {
        HasBeenPickedUp = false;
        anim.SetTrigger("BringBack");
        canPickup = true;
        collider.enabled = true;
    }

    public void Fade()
    {
        anim.SetTrigger("Fade");
        collider.enabled = false;
    }
}
