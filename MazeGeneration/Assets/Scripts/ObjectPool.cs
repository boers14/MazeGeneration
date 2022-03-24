using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public Transform objectForPool = null;

    [System.NonSerialized]
    public List<Transform> objectPool = new List<Transform>(), activeObjects = new List<Transform>();

    // Base class for re-using a single object for object pool
    // TODO: Make object pool that supports multiple objects
    public virtual void Start()
    {
        AddObjectsToPool(100);
    }

    // Add a certain amount of objects to the pool
    public void AddObjectsToPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Transform newObject = Instantiate(objectForPool, transform);
            newObject.gameObject.SetActive(false);
            objectPool.Add(newObject);
        }
    }

    // Return all active objects to the pool 
    public void ReturnAllObjectsToPool()
    {
        for (int i = activeObjects.Count - 1; i >= 0; i--)
        {
            activeObjects[i].gameObject.SetActive(false);
            objectPool.Add(activeObjects[i]);
        }
        activeObjects.Clear();
    }

    // Return a single object to the pool
    public void ReturnObjectToPool(Transform returnedObject)
    {
        activeObjects.Remove(returnedObject);
        objectPool.Add(returnedObject);
        returnedObject.gameObject.SetActive(false);
    }

    // Retrieve an object from the pool
    public Transform RetrieveObjectFromPool()
    {
        // Add objects to the pool if pool is empty
        if (objectPool.Count == 0)
        {
            AddObjectsToPool(5);
        }

        Transform newObject = objectPool[0];
        objectPool.Remove(newObject);

        newObject.gameObject.SetActive(true);
        activeObjects.Add(newObject);

        return newObject;
    }
}
