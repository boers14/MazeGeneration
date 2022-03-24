using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField]
    private float shootInterval = 0.1f, damage = 10, range = 4, amountOfBullets = 50, reloadTime = 1f;

    private float shootCooldown = 0, currentAmountOfBullets = 0;

    private bool isShooting = false, reloading = false;

    [SerializeField]
    private ParticleSystem gunSmoke = null;

    private ParticleSystem gunBullets = null;

    [SerializeField]
    private List<ParticleSystem> typeOfExplosions = new List<ParticleSystem>();

    [System.NonSerialized]
    public List<ParticleSystem> explosions = new List<ParticleSystem>();

    private new PlayerCamera camera = null;

    [SerializeField]
    private LayerMask layerToIgnore = 0;

    [SerializeField]
    private AudioSource gunShot = null, gunReload = null;

    // Is object pool of multiple particle effects
    private void Start()
    {
        // Inverse layer
        layerToIgnore = ~layerToIgnore;
        currentAmountOfBullets = amountOfBullets;
        BulletCounter.instance.UpdateValue((int)amountOfBullets);
        gunBullets = GetComponent<ParticleSystem>();

        for (int i = 0; i < typeOfExplosions.Count; i++)
        {
            AddExplosionEffectToList(typeOfExplosions[i], 25);
        }
        camera = Camera.main.GetComponent<PlayerCamera>();
    }

    // Handles shooting when left mouse button is pressed
    private void Update()
    {
        // Return if the player is reloading
        if (reloading) { return; }

        // Can shoot only when cooldown is below 0
        if (Input.GetMouseButton(0) && shootCooldown <= 0)
        {
            // Use random pitch for gun
            gunShot.pitch = 1 + Random.Range(-0.2f, 0.2f);
            gunShot.Play();

            // Start playing particles if player was not shooting
            if (!isShooting)
            {
                StartCoroutine(StartGunShootingParticleSystems());
            }

            // Shoot raycast from gun that ignores IgnoreRaycast and pickup layer
            shootCooldown = shootInterval;
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, range, layerToIgnore))
            {
                // Have different explosion type based on what is hit
                GunExplosions.ExplosionType explosionType = GunExplosions.ExplosionType.Wall;
                if (hit.transform.tag == "Enemy")
                {
                    explosionType = GunExplosions.ExplosionType.Blood;
                    hit.transform.GetComponentInParent<Enemy>().TakeDamage(damage);
                } else if (hit.transform.tag == "Floor")
                {
                    explosionType = GunExplosions.ExplosionType.Floor;
                } else if (hit.transform.tag == "Wall")
                {
                    explosionType = GunExplosions.ExplosionType.Wall;
                }

                // Create explosion on the hit position
                StartCoroutine(CreateParticleSystemOnHitPosition(hit.distance / 15, hit.point, explosionType));

                // Update bullet count
                currentAmountOfBullets--;
                BulletCounter.instance.UpdateValue(-1);
                // Reload if there are no more bullets
                if (currentAmountOfBullets <= 0)
                {
                    StartCoroutine(StartReloading());
                }
            }
        } else if (!Input.GetMouseButton(0))
        {
            // Stop particles if the player was shooting
            if (isShooting)
            {
                StopShooting();
            }
        }

        // Reload
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(StartReloading());
        }

        shootCooldown -= Time.deltaTime;
    }

    // Add explosion effect to pool
    private void AddExplosionEffectToList(ParticleSystem particleSystemPrefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            ParticleSystem particleSystem = Instantiate(particleSystemPrefab);
            particleSystem.gameObject.SetActive(false);
            particleSystem.GetComponent<GunExplosions>().player = this;
            explosions.Add(particleSystem);
        }
    }

    // Start gun particles effects
    private IEnumerator StartGunShootingParticleSystems()
    {
        camera.camShake = true;
        isShooting = true;
        gunBullets.Play();
        yield return new WaitForSeconds(0.05f);
        gunSmoke.Play();
    }

    // Create given type of explosion effect on given position based on the distance from the player
    private IEnumerator CreateParticleSystemOnHitPosition(float distanceTime, Vector3 explosionPos, 
        GunExplosions.ExplosionType explosionType)
    {
        ParticleSystem newExplosion = ReturnParticleSystemBasedOnExplosionType(explosions, explosionType);

        // If no explosion was found in pool add explosions of given type to pool
        if (!newExplosion)
        {
            AddExplosionEffectToList(ReturnParticleSystemBasedOnExplosionType(typeOfExplosions, explosionType), 5);
            newExplosion = ReturnParticleSystemBasedOnExplosionType(explosions, explosionType);
        }

        yield return new WaitForSeconds(distanceTime);

        explosions.Remove(newExplosion);
        newExplosion.transform.LookAt(camera.transform);
        newExplosion.transform.position = explosionPos;
        newExplosion.gameObject.SetActive(true);
        newExplosion.Play();
        StartCoroutine(newExplosion.GetComponent<GunExplosions>().ReturnExplosionToList());
    }

    // Return explosion from given list based on type
    private ParticleSystem ReturnParticleSystemBasedOnExplosionType(List<ParticleSystem> particleSystems,
        GunExplosions.ExplosionType explosionType)
    {
        return particleSystems.Find(explosion => explosion.GetComponent<GunExplosions>().explosionType == explosionType);
    }

    // Stop shooting effects and set reload to true, set amount of bullets to max amount, update text with it
    private IEnumerator StartReloading()
    {
        gunReload.Play();
        StopShooting();
        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        BulletCounter.instance.UpdateValue((int)(amountOfBullets - currentAmountOfBullets));
        currentAmountOfBullets = amountOfBullets;
        reloading = false;
        shootCooldown = 0;
    }

    // Stop shooting effects
    private void StopShooting()
    {
        camera.camShake = false;
        isShooting = false;
        gunBullets.Stop();
        gunSmoke.Stop();
    }

    // Increase bullet max capacity, reload
    public void AddBullets(float bulletIncrease)
    {
        amountOfBullets += bulletIncrease;
        StartCoroutine(StartReloading());
    }

    // Increase damage done
    public void IncreasePower(float damageIncrease)
    {
        damage += damageIncrease;
    }
}
