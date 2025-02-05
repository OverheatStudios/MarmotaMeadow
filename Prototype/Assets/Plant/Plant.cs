using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    private enum PlantState
    {
        Normal,
        Tealed,
        Planted,
        Waterd
    }
    
    [SerializeField] PlantState state = PlantState.Normal;
    [SerializeField] private Seeds seed = null;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeState(BaseItem item)
    {
        if (item.name == "hoe" && state == PlantState.Normal)
        {
            state = PlantState.Tealed;
        }else if (item is Seeds  && state == PlantState.Tealed)
        {
            state = PlantState.Planted;
            seed = (Seeds)item;
        }else if (item.name == "watering can" && state == PlantState.Planted)
        {
            state = PlantState.Waterd;
        }
    }
}
