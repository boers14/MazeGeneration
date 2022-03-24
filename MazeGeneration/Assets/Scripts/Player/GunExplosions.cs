using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunExplosions : MonoBehaviour
{
    [System.NonSerialized]
    public PlayerShooting player = null;

    private float explosionTime = 0;

    private new ParticleSystem particleSystem = null;

    public enum ExplosionType
    {
        Floor,
        Wall,
        Blood
    }

    public ExplosionType explosionType = ExplosionType.Floor;

    // Set vars
    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        explosionTime = particleSystem.main.startLifetime.constant;
    }

    // Return explosion to the objectpool after it played
    public IEnumerator ReturnExplosionToList()
    {
        yield return new WaitForSeconds(explosionTime * 3);
        gameObject.SetActive(false);
        player.explosions.Add(particleSystem);
    }
}
