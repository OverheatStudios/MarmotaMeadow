using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    [System.Serializable]
    public class PoolObject
    {
        public string key;
        public GameObject prefab;
        public int amountToSpawn;
    }

    [SerializeField] private List<PoolObject> objectsToSpawn;
    private Dictionary<string, List<GameObject>> objectPool = new Dictionary<string, List<GameObject>>();

    void Start()
    {
        CreatePooledObjects();
    }

    void Update()
    {
        // Debug logs can be added here if needed
    }

    void CreatePooledObjects()
    {
        foreach (var poolObject in objectsToSpawn)
        {
            List<GameObject> poolList = new List<GameObject>();
            for (int j = 0; j < poolObject.amountToSpawn; ++j)
            {
                GameObject obj = Instantiate(poolObject.prefab, Vector3.zero, Quaternion.identity);
                obj.SetActive(false);
                poolList.Add(obj);
            }
            objectPool.Add(poolObject.key, poolList);
        }
    }
    
    public GameObject TakeObjectOut(string objectToTakeOut)
    {
        if (objectPool.TryGetValue(objectToTakeOut, out List<GameObject> objectsOfType))
        {
            if (objectsOfType.Count > 0)
            {
                GameObject objectPulledOut = objectsOfType[^1];
                objectsOfType.RemoveAt(objectsOfType.Count - 1);
                objectPulledOut.SetActive(true);
                return objectPulledOut;
            }
        }
        return null;
    }
    
    public void PutObjectBack(string objectToTakeOut, GameObject objectPulledOut)
    {
        if (objectPool.TryGetValue(objectToTakeOut, out List<GameObject> objectsOfType))
        {
            if (objectsOfType.Count == 0)
            {
                return;
            }
            
            if (!objectsOfType.Contains(objectPulledOut))
            {
                objectPulledOut.transform.position = Vector3.zero;
                objectPulledOut.SetActive(false);
                objectsOfType.Add(objectPulledOut);
            }
        }
    }
}
