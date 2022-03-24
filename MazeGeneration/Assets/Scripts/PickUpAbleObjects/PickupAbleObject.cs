using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class PickupAbleObject : MonoBehaviour
{
    [SerializeField]
    private float boxOpenedTime = 0.5f;

    private Animator animator = null;

    private BoxCollider[] colliders = null;

    private new Rigidbody rigidbody = null;

    [System.NonSerialized]
    public bool isOpened = false;

    [SerializeField]
    private string powerUpExplanation = "";

    public PickUpSpawner.PickupAbleObjectType type = PickUpSpawner.PickupAbleObjectType.HealthRegain;

    private AudioSource pickUpSoundEffect = null;

    private void Start()
    {
        Instantiate();
    }

    // Set vars
    public virtual void Instantiate()
    {
        colliders = GetComponentsInChildren<BoxCollider>();
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        pickUpSoundEffect = GetComponent<AudioSource>();
        rigidbody.freezeRotation = true;
    }

    // Start playing animation when collide with player
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player" && !isOpened)
        {
            StartCoroutine(PlayAnimation());
        }
    }

    // Play animation/ sound. Cant collide with player anymore and stop movement
    private IEnumerator PlayAnimation()
    {
        pickUpSoundEffect.Play();
        isOpened = true;
        SwitchCollidebleStatus(false);
        rigidbody.velocity = Vector3.zero;
        animator.SetBool("isOpening", true);
        yield return new WaitForEndOfFrame();
        StartCoroutine(PauzeAnimation());
    }

    // Pauze animation at the halfway point to grant effect and show text
    private IEnumerator PauzeAnimation()
    {
        yield return new WaitForSeconds(1f);
        animator.speed = 0;
        GrantEffect();
        PickUpSpawner.instance.ShowPowerupEffect(powerUpExplanation);
        StartCoroutine(ContinueAnimation());
    }

    // After given amount of time of being open, close again
    private IEnumerator ContinueAnimation()
    {
        yield return new WaitForSeconds(boxOpenedTime);
        animator.speed = 1;
        StartCoroutine(EndAnimation());
    }

    // Stop animation and make object collideble again. Return to pool
    private IEnumerator EndAnimation()
    {
        yield return new WaitForSeconds(1f);
        animator.SetBool("isOpening", false);
        PickUpSpawner.instance.ReturnObjectToPool(this);
        SwitchCollidebleStatus(true);
    }

    // Switch if objects can collide with object
    private void SwitchCollidebleStatus(bool status)
    {
        foreach (BoxCollider collider in colliders)
        {
            collider.isTrigger = !status;
        }
        rigidbody.useGravity = status;
    }

    // Effect of pickup
    public virtual void GrantEffect()
    {

    }
}
