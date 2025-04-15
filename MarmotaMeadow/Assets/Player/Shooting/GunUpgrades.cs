using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunUpgrades : MonoBehaviour
{
    public int GetExtraBullets()
    {
        return 10;
    }

    public bool ShouldConsumeAmmo()
    {
        float ammoNoConsumeChance = 1.0f;
        return Random.Range(0.0f, 1.0f) > ammoNoConsumeChance;
    }

    public float GetReloadSpeedScalar()
    {
        return 0.1f;
    }
}
