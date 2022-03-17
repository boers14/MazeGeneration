using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public Transform objectForPool = null;

    [System.NonSerialized]
    public List<Transform> objectPool = new List<Transform>(), activeObjects = new List<Transform>();

    public virtual void Start()
    {
        AddObjectsToPool(100);
    }

    public void AddObjectsToPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Transform newWall = Instantiate(objectForPool, transform);
            newWall.gameObject.SetActive(false);
            objectPool.Add(newWall);
        }
    }

    public void ReturnAllObjectsToPool()
    {
        for (int i = activeObjects.Count - 1; i >= 0; i--)
        {
            activeObjects[i].gameObject.SetActive(false);
            objectPool.Add(activeObjects[i]);
        }
        activeObjects.Clear();
    }
}
