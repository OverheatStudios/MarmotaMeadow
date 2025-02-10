using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
    
public class Plant : MonoBehaviour
{
    private enum PlantState
    {
        Normal,
        Tealed,
        Planted,
        Completed
    }
    
    [SerializeField] private PlantState state = PlantState.Normal;
    [SerializeField] private Seeds m_seed = null;
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private TextMeshProUGUI growthText;
    [SerializeField] private Image image;
    [SerializeField] private float growthTimer = 0f;
    [SerializeField] private float multiplier = 1.0f;
    [SerializeField] private GameObject cropToSpawn;
    [SerializeField] private float throwAngle = 45f; // Angle of the throw (in degrees)
    [SerializeField] private float throwDistance = 5f;
    [SerializeField] private GameObject cropToSpawnLocation;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool ChangeState(BaseItem item)
    {
        if (item.name == "hoe" && state == PlantState.Normal)
        {
            state = PlantState.Tealed;
            stateText.text = state.ToString();
            gameObject.GetComponent<Renderer>().material.color = new Color32(244, 237, 22 , 1);
            return true;
        }else if (item is Seeds  && state == PlantState.Tealed)
        {
            //changing state
            state = PlantState.Planted;
            stateText.text = state.ToString();
            //adding the seed
            m_seed = (Seeds)item;
            image.sprite = m_seed.ReturnImage();
            growthTimer = m_seed.ReturnGrowDuration();
            StartCoroutine(nameof(CountdownRoutine));
            //some visual feedback
            gameObject.GetComponent<Renderer>().material.color = Color.yellow;
            return true;
        }else if (item.name == "watering can" && state == PlantState.Planted)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.blue;
            return true;
        }else if (item.name == "harvesting tool" && state == PlantState.Completed)
        {
            state = PlantState.Normal;
            stateText.text = state.ToString();
            image.sprite = null;
            HarvestCrop();
        }
        return false;
    }
    
    private IEnumerator CountdownRoutine()
    {
        while (growthTimer > 0)
        {
            growthText.text = "Grow Timer: "+ growthTimer.ToString();
            yield return new WaitForSeconds(1f);
            growthTimer -= 1f;
        }

        state = PlantState.Completed;
        stateText.text = state.ToString();
        StopAllCoroutines();
    }
    
    void HarvestCrop()
    {
        GameObject spawnedItem = Instantiate(cropToSpawn, cropToSpawnLocation.transform.position, Quaternion.identity);
        
        spawnedItem.GetComponent<SpawnedItem>().SetItem(m_seed);

        // Apply force to throw the item in an arch
        Rigidbody itemRb = spawnedItem.GetComponent<Rigidbody>();
        if (itemRb != null)
        {
            Vector3 throwDirection = CalculateArchVelocity(throwAngle, throwDistance);
            itemRb.velocity = throwDirection;
        }
    }
    
    Vector3 CalculateArchVelocity(float angle, float distance)
    {
        // Convert the angle to radians
        float angleRad = angle * Mathf.Deg2Rad;

        // Calculate the initial velocity using projectile motion formulas
        float gravity = Physics.gravity.magnitude;
        float initialVelocity = Mathf.Sqrt((distance * gravity) / Mathf.Sin(2 * angleRad));

        // Break the velocity into horizontal and vertical components
        float horizontalVelocity = initialVelocity * Mathf.Cos(angleRad);
        float verticalVelocity = initialVelocity * Mathf.Sin(angleRad);

        // Create a random horizontal direction
        Vector3 horizontalDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;

        // Combine the horizontal and vertical components
        Vector3 throwDirection = horizontalDirection * horizontalVelocity + Vector3.up * verticalVelocity;

        return throwDirection;
    }

}
