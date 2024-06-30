using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject[] blockPrefabs; // Array to hold the block prefabs
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    public void Initialize()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // Initialize the pool for each block prefab
        foreach (var prefab in blockPrefabs)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            poolDictionary.Add(prefab.name, objectPool);
        }
    }

    public GameObject GetFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        GameObject objectToReuse;
        if (poolDictionary[tag].Count > 0)
        {
            objectToReuse = poolDictionary[tag].Dequeue();
            objectToReuse.SetActive(true);
        }
        else
        {
            objectToReuse = Instantiate(blockPrefabs[GetPrefabIndexByName(tag)]);
        }

        objectToReuse.transform.position = position;
        objectToReuse.transform.rotation = rotation;
        return objectToReuse;
    }

    public void ReturnToPool(GameObject objectToReturn)
    {
        objectToReturn.SetActive(false);
        poolDictionary[objectToReturn.tag].Enqueue(objectToReturn);
    }

    private int GetPrefabIndexByName(string name)
    {
        for (int i = 0; i < blockPrefabs.Length; i++)
        {
            if (blockPrefabs[i].name == name)
                return i;
        }
        return -1;
    }
}