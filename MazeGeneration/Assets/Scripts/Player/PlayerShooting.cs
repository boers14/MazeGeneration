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

    private void Start()
    {
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

    private void Update()
    {
        if (reloading) { return; }
        if (Input.GetMouseButton(0) && shootCooldown <= 0)
        {
            gunShot.pitch = 1 + Random.Range(-0.2f, 0.2f);
            gunShot.Play();
            if (!isShooting)
            {
                StartCoroutine(StartGunShootingParticleSystems());
            }

            shootCooldown = shootInterval;
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, range, layerToIgnore))
            {
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

                StartCoroutine(CreateParticleSystemOnHitPosition(hit.distance / 15, hit.point, explosionType));

                currentAmountOfBullets--;
                BulletCounter.instance.UpdateValue(-1);
                if (currentAmountOfBullets <= 0)
                {
                    StartCoroutine(StartReloading());
                }
            }
        } else if (!Input.GetMouseButton(0))
        {
            if (isShooting)
            {
                StopShooting();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(StartReloading());
        }

        shootCooldown -= Time.deltaTime;
    }

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

    private IEnumerator StartGunShootingParticleSystems()
    {
        camera.camShake = true;
        isShooting = true;
        gunBullets.Play();
        yield return new WaitForSeconds(0.05f);
        gunSmoke.Play();
    }

    private IEnumerator CreateParticleSystemOnHitPosition(float distanceTime, Vector3 explosionPos, 
        GunExplosions.ExplosionType explosionType)
    {
        ParticleSystem newExplosion = ReturnParticleSystemBasedOnExplosionType(explosions, explosionType);

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

    private ParticleSystem ReturnParticleSystemBasedOnExplosionType(List<ParticleSystem> particleSystems,
        GunExplosions.ExplosionType explosionType)
    {
        return particleSystems.Find(explosion => explosion.GetComponent<GunExplosions>().explosionType == explosionType);
    }

    private IEnumerator StartReloading()
    {
        gunReload.Play();
        StopShooting();
        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        BulletCounter.instance.UpdateValue((int)(amountOfBullets - currentAmountOfBullets));
        currentAmountOfBullets = amountOfBullets;
        reloading = false;
    }

    private void StopShooting()
    {
        camera.camShake = false;
        isShooting = false;
        gunBullets.Stop();
        gunSmoke.Stop();
    }

    public void AddBullets(float bulletIncrease)
    {
        amountOfBullets += bulletIncrease;
        StartCoroutine(StartReloading());
    }

    public void IncreasePower(float damageIncrease)
    {
        damage += damageIncrease;
    }
}
