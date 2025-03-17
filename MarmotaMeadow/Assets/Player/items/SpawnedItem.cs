using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedItem : MonoBehaviour
{
    [SerializeField] private BaseItem _item;

    public void SetItem(BaseItem item)
    {
        _item = item;
        GetComponent<SpriteRenderer>().sprite = _item.ReturnImage();
    }

    public BaseItem ReturnItem()
    {
        return _item;
    }
}
