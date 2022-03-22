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

    private void Awake()
    {
        transform.SetParent(null);
        renderer = GetComponent<MeshRenderer>();

        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = explosionRadius;
        sphereCollider.enabled = false;

        particleSystem = GetComponent<ParticleSystem>();
    }

    public IEnumerator StartCountDown()
    {
        renderer.enabled = true;
        yield return new WaitForSeconds(timeTillExplosion);
        StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {
        renderer.enabled = false;
        particleSystem.Play();
        sphereCollider.enabled = true;
        yield return new WaitForSeconds(explosionDuration);
        sphereCollider.enabled = false;
        PlayerGrenadePool.instance.ReturnObjectToPool(transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            other.transform.GetComponentInParent<Enemy>().TakeDamage(damageDone);
        }
    }
}
