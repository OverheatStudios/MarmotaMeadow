using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pair<K, V>
{
    public K First;
    public V Second;

    public Pair(K first, V second)
    {
        this.First = first;
        this.Second = second;
    }
}
