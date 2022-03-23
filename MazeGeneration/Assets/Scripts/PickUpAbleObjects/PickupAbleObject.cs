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

    public PickUpSpawner.PickupAbleObjectType type = PickUpSpawner.PickupAbleObjectType.HealthRegain;

    public virtual void Instantiate()
    {
        colliders = GetComponentsInChildren<BoxCollider>();
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player" && !isOpened)
        {
            StartCoroutine(PlayAnimation());
        }
    }

    private IEnumerator PlayAnimation()
    {
        isOpened = true;
        SwitchCollidebleStatus(false);
        rigidbody.velocity = Vector3.zero;
        animator.SetBool("isOpening", true);
        yield return new WaitForEndOfFrame();
        StartCoroutine(PauzeAnimation());
    }

    private IEnumerator PauzeAnimation()
    {
        yield return new WaitForSeconds(1f);
        animator.speed = 0;
        GrantEffect();
        StartCoroutine(ContinueAnimation());
    }

    private IEnumerator ContinueAnimation()
    {
        yield return new WaitForSeconds(boxOpenedTime);
        animator.speed = 1;
        StartCoroutine(EndAnimation());
    }

    private IEnumerator EndAnimation()
    {
        yield return new WaitForSeconds(1f);
        animator.SetBool("isOpening", false);
        PickUpSpawner.instance.ReturnObjectToPool(this);
        Destroy(gameObject);
        SwitchCollidebleStatus(true);
    }

    private void SwitchCollidebleStatus(bool status)
    {
        foreach (BoxCollider collider in colliders)
        {
            collider.isTrigger = !status;
        }
        rigidbody.useGravity = status;
    }

    public virtual void GrantEffect()
    {

    }
}
