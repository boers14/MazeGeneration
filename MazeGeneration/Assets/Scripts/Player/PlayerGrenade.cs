using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrenade : MonoBehaviour
{
    [SerializeField]
    private float timeTillExplosion = 1.5f, explosionRadius = 2.5f, explosionDuration = 0.5f, damageDone = 30;

    private new ParticleSystem particleSystem = null;

    private SphereCollider sphereCollider = null;

    private new MeshRenderer renderer = null;

    private AudioSource explosionSound = null;

    // Set vars
    private void Awake()
    {
        transform.SetParent(null);
        renderer = GetComponent<MeshRenderer>();

        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = explosionRadius;
        sphereCollider.enabled = false;

        particleSystem = GetComponent<ParticleSystem>();
        explosionSound = GetComponent<AudioSource>();
    }

    // Show grenade and start explosion after given time
    public IEnumerator StartCountDown()
    {
        renderer.enabled = true;
        yield return new WaitForSeconds(timeTillExplosion);
        StartCoroutine(Explode());
    }

    // Deal damage for the given amount of time
    private IEnumerator Explode()
    {
        explosionSound.Play();
        renderer.enabled = false;
        particleSystem.Play();
        sphereCollider.enabled = true;
        yield return new WaitForSeconds(explosionDuration);
        sphereCollider.enabled = false;
        if (explosionDuration < explosionSound.clip.length)
        {
            StartCoroutine(ReturnObjectToPool());
        }
    }

    // Return object to pool after sounds effect is done
    private IEnumerator ReturnObjectToPool()
    {
        yield return new WaitForSeconds(explosionSound.clip.length - explosionDuration);
        PlayerGrenadePool.instance.ReturnObjectToPool(transform);
    }

    // For every collider on the enemy deal damage (damageDone * 16)
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            other.transform.GetComponentInParent<Enemy>().TakeDamage(damageDone);
        }
    }

    // Increase grenade damage done
    public void IncreasePower(float extraDamage)
    {
        damageDone += extraDamage;
    }
}
