using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrenadePool : ObjectPool
{
    [SerializeField]
    private float throwInterval = 0.5f, startAmountOfGrenades = 5, throwStrenght = 10;

    private float throwTimer = 0, currentAmountOfGrenades = 0;

    public static PlayerGrenadePool instance = null;

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

        currentAmountOfGrenades = startAmountOfGrenades;
        base.Start();
    }

    private void Update()
    {
        if (throwTimer <= 0 && currentAmountOfGrenades > 0 && Input.GetKeyDown(KeyCode.E))
        {
            currentAmountOfGrenades--;
            throwTimer = throwInterval;
            PlayerGrenade newGrenade = RetrieveObjectFromPool().GetComponent<PlayerGrenade>();
            newGrenade.transform.position = transform.position;
            newGrenade.GetComponent<Rigidbody>().velocity = Vector3.zero;
            newGrenade.GetComponent<Rigidbody>().AddForce((transform.forward + transform.up * 0.75f) * throwStrenght);
            StartCoroutine(newGrenade.StartCountDown());
        }

        throwTimer -= Time.deltaTime;
    }
}
