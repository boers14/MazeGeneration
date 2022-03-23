using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSpawner : MonoBehaviour
{
    public static PickUpSpawner instance = null;

    [System.NonSerialized]
    public float amountOfHealthIncreased = 0;

    [SerializeField]
    private float spawnPickupInterval = 30;

    [SerializeField]
    private List<PickupAbleObject> possiblePickupAbleObjects = new List<PickupAbleObject>();

    private List<PickupAbleObject> activeObjects = new List<PickupAbleObject>(), objectPool = new List<PickupAbleObject>();

    public enum PickupAbleObjectType
    {
        HealthRegain,
        AmmoIncrease,
        PowerIncrease
    }

    private void Start()
    {
        if (!instance)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
            return;
        }

        for (int i = 0; i < possiblePickupAbleObjects.Count; i++)
        {
            AddObjectsToPool(15, possiblePickupAbleObjects[i]);
        }
    }

    private void AddObjectsToPool(int count, PickupAbleObject pickupAbleObject)
    {
        for (int i = 0; i < count; i++)
        {
            PickupAbleObject newObject = Instantiate(pickupAbleObject, transform);
            newObject.Instantiate();
            newObject.gameObject.SetActive(false);
            objectPool.Add(newObject);
        }
    }

    public void ReturnObjectToPool(PickupAbleObject pickupAbleObject)
    {
        activeObjects.Remove(pickupAbleObject);
        objectPool.Add(pickupAbleObject);
        pickupAbleObject.gameObject.SetActive(false);
    }

    public IEnumerator SpawnPickUps()
    {
        PickupAbleObjectType type = (PickupAbleObjectType)Random.Range(0, System.Enum.GetNames(typeof(PickupAbleObjectType)).Length);
        PickupAbleObject newObject = RetrieveObjectFromPoolBasedOnType(type);
        newObject.transform.position = EnemyManager.instance.CreatePositionForEnemy() + new Vector3(0, 0.5f, 1);
        newObject.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
        newObject.isOpened = false;

        yield return new WaitForSeconds(spawnPickupInterval);
        StartCoroutine(SpawnPickUps());
    }

    private PickupAbleObject RetrieveObjectFromPoolBasedOnType(PickupAbleObjectType type)
    {
        PickupAbleObject newObject = FindObjectBasedOnType(objectPool, type);
        if (!newObject)
        {
            AddObjectsToPool(5, FindObjectBasedOnType(possiblePickupAbleObjects, type));
            newObject = FindObjectBasedOnType(objectPool, type);
        }

        objectPool.Remove(newObject);
        activeObjects.Add(newObject);
        newObject.gameObject.SetActive(true);
        return newObject;
    }

    private PickupAbleObject FindObjectBasedOnType(List<PickupAbleObject> pickupAbleObjects, PickupAbleObjectType type)
    {
        return pickupAbleObjects.Find(obj => obj.type == type);
    }
}
