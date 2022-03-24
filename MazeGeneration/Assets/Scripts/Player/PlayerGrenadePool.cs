using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrenadePool : ObjectPool
{
    [SerializeField]
    private float throwInterval = 0.5f, startAmountOfGrenades = 5, throwStrenght = 10;

    private float throwTimer = 0, currentAmountOfGrenades = 0;

    // Is singleton
    public static PlayerGrenadePool instance = null;

    private Transform camTransform = null;

    public override void Start()
    {
        if (!instance)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
            return;
        }

        // Set grenades to start amount
        currentAmountOfGrenades = startAmountOfGrenades;
        GrenadeCounter.instance.UpdateValue((int)startAmountOfGrenades);
        camTransform = Camera.main.transform;
        base.Start();
    }

    // If possible create a grenade for the player to throw
    private void Update()
    {
        if (throwTimer <= 0 && currentAmountOfGrenades > 0 && Input.GetKeyDown(KeyCode.E))
        {
            GrenadeCounter.instance.UpdateValue(-1);
            currentAmountOfGrenades--;
            throwTimer = throwInterval;
            PlayerGrenade newGrenade = RetrieveObjectFromPool().GetComponent<PlayerGrenade>();
            newGrenade.transform.position = transform.position;
            newGrenade.GetComponent<Rigidbody>().velocity = Vector3.zero;
            newGrenade.GetComponent<Rigidbody>().AddForce((camTransform.forward + camTransform.up * 0.75f) * throwStrenght);
            StartCoroutine(newGrenade.StartCountDown());
        }

        throwTimer -= Time.deltaTime;
    }

    // Add grenades and update counter
    public void AddGrenades(float addedGrenades)
    {
        currentAmountOfGrenades += addedGrenades;
        GrenadeCounter.instance.UpdateValue((int)addedGrenades);
    }

    // Go over both lists to increase power
    public void IncreaseGrenadePower(float damageIncrease)
    {
        IncreaseListPower(objectPool, damageIncrease);
        IncreaseListPower(activeObjects, damageIncrease);
    }

    // Increase damage done from grenades
    private void IncreaseListPower(List<Transform> objects, float damageIncrease)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].GetComponent<PlayerGrenade>().IncreasePower(damageIncrease);
        }
    }
}
