using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SpawnDeadGroundHogs : MonoBehaviour
{
    
    [SerializeField] private GameObject spawnDeadGroundHogPrefab;
    [SerializeField] private ScrObjGlobalData scrObjGlobalData;
    [SerializeField] private GameObject[] deadGroundHogPositions;
    
    // Start is called before the first frame update
    void Start()
    {
        if (scrObjGlobalData && scrObjGlobalData.GetData().GetNightCounterPossiblyNegative() >= 0)
        {
            switch (scrObjGlobalData.GetData().CurrentHealth)
            {
                case 100:
                    SpawnDeadGroundHog(3);
                    break;
                case 80:
                    SpawnDeadGroundHog(2);
                    break;
                case 60:
                    SpawnDeadGroundHog(2);
                    break;
                case 40:
                    SpawnDeadGroundHog(2);
                    break;
                case 20:
                    SpawnDeadGroundHog(1);
                    break;
            }
        }
    }

    void SpawnDeadGroundHog(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Instantiate(spawnDeadGroundHogPrefab, deadGroundHogPositions[i].transform.position, Quaternion.identity);
        }
    }
}
