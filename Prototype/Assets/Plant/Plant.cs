using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;   
    
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
    [SerializeField] private float maxGrowthTimer;
    [SerializeField] private float multiplier = 1.0f;
    [SerializeField] private GameObject cropToSpawn;
    [SerializeField] private float throwAngle = 45f; // Angle of the throw (in degrees)
    [SerializeField] private float throwDistance = 5f;
    [SerializeField] private GameObject cropToSpawnLocation;
    
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer spriteRenderer2;
    
    [SerializeField] private UnityEvent gamePausedEvent;
    
    
    // Start is called before the first frame update
    void Start()
    {
        growthTimer = maxGrowthTimer;
        spriteRenderer.sprite = null;
        spriteRenderer2.sprite = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (growthTimer < maxGrowthTimer/2 && state == PlantState.Planted)
        {
            spriteRenderer.sprite = m_seed.ReturnGrowingSprite();
            spriteRenderer2.sprite = m_seed.ReturnGrowingSprite();
        }
    }

    public bool ChangeState(InventoryItem item)
    {

        if (item.item.name == "hoe" && state == PlantState.Normal)
        {
            state = PlantState.Tealed;
            stateText.text = state.ToString();
            multiplier += item.ReturnMultiplier();
            return true;
        }else if (item.item is Seeds seeds  && state == PlantState.Tealed)
        {
            //changing state
            state = PlantState.Planted;
            stateText.text = state.ToString();
            //adding the seed
            m_seed = seeds;
            image.sprite = m_seed.ReturnImage();
            growthTimer = m_seed.ReturnGrowDuration();
            maxGrowthTimer = m_seed.ReturnGrowDuration();
            StartCoroutine(nameof(CountdownRoutine));
            multiplier = m_seed.ReturnAmount();
            //some visual feedback
            spriteRenderer.sprite = m_seed.ReturnPlantedSprite();
            spriteRenderer2.sprite = m_seed.ReturnPlantedSprite();
            return true;
        }else if (item.item.name == "watering can" && state == PlantState.Planted)
        {
            return true;
        }else if (item.item.name == "harvesting tool" && state == PlantState.Completed)
        {
            multiplier += item.ReturnMultiplier();
            state = PlantState.Normal;
            stateText.text = state.ToString();
            image.sprite = null;
            spriteRenderer.sprite = null;
            spriteRenderer2.sprite = null;
            HarvestCrop();
            return true;
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
        spriteRenderer.sprite = m_seed.ReturnFinishedSprite();
        spriteRenderer2.sprite = m_seed.ReturnFinishedSprite();
        StopAllCoroutines();
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    void HarvestCrop()
    {
        for (int i = 0; i < multiplier; i++)
        {
            GameObject spawnedItem = Instantiate(cropToSpawn, cropToSpawnLocation.transform.position, Quaternion.identity);
        
            spawnedItem.GetComponent<SpawnedItem>().SetItem(m_seed.ReturnCrop());

            // Apply force to throw the item in an arch
            Rigidbody itemRb = spawnedItem.GetComponent<Rigidbody>();
            if (itemRb != null)
            {
                Vector3 throwDirection = CalculateArchVelocity(throwAngle, throwDistance);
                itemRb.velocity = throwDirection;
            }
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
