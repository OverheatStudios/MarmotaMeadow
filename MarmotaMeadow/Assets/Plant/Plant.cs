using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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

    [SerializeField] private Billboard m_billboard;
    
    [SerializeField] private UnityEvent gamePausedEvent;
    
    [SerializeField] private MovementScript playerMovement;
    
    [SerializeField] private ObjectPooling objectPool;
    [SerializeField] private TutorialManager tutorialManager;
    
    [SerializeField] private GameObject tealedGround;
    [SerializeField] private GameObject untealedGround;

    [SerializeField] private GameObject m_tillingParticleSystem;
    [SerializeField] private Vector3 m_tillingParticlesOffset = new Vector3(0, 0.15f, 0);

    [SerializeField] private GameObject m_harvestingParticleSystem;
    [SerializeField] private Vector3 m_harvestingParticlesOffset = new Vector3(0, 0.15f, 0);
    
    
    
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform targetCameraPosition;
    [SerializeField] private float duration;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    private bool isCameraInPosition = false;
    
    
    [Header("MiniGame")]
    [SerializeField] private GameObject miniGame;
    [SerializeField] private GameObject line;
    [SerializeField] private bool finishedMiniGame = true;

    
    // Start is called before the first frame update
    void Start()
    {
        growthTimer = maxGrowthTimer;
        m_billboard.SetSprite(null);
        tealedGround.SetActive(false);
        untealedGround.SetActive(true);
        if (SceneManager.GetActiveScene().name == "NightScene")
        {
            return;
        }
        objectPool = GameObject.FindGameObjectWithTag("ObjectPool").GetComponent<ObjectPooling>();
        tutorialManager = GameObject.FindGameObjectWithTag("TutorialManager").GetComponent<TutorialManager>();
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<MovementScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (growthTimer < maxGrowthTimer/2 && state == PlantState.Planted)
        {
            m_billboard.SetSprite(m_seed.ReturnGrowingSprite());
        }
        
        if (isCameraInPosition && Input.GetMouseButtonDown(1)) // Right mouse button to reset camera
        {
            finishedMiniGame = true;
            StartCoroutine(MoveCamera(originalCameraPosition, originalCameraRotation, 1.5f));
            isCameraInPosition = false;
        }
    }

    public bool ChangeState(InventoryItem item)
    {
        if (item.item.name == "hoe" && state == PlantState.Normal && (tutorialManager.GetTutorialData().step == 3 || tutorialManager.GetTutorialData().isFinsihed))
        {
            state = PlantState.Tealed;
            stateText.text = state.ToString();
            multiplier += item.ReturnMultiplier();
            tealedGround.SetActive(true);
            untealedGround.SetActive(false);

            GameObject particles = Instantiate(m_tillingParticleSystem);
            particles.transform.SetParent(transform, false);
            particles.transform.position += m_tillingParticlesOffset;
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
            m_billboard.SetSprite(m_seed.ReturnPlantedSprite());
            return true;
        }else if (item.item.name == "watering can" && state == PlantState.Planted)
        {
            return true;
        }else if (item.item.name == "harvesting tool" && state == PlantState.Completed && !isCameraInPosition)
        {
            playerMovement.enabled = false;
            originalCameraPosition = mainCamera.transform.position;
            originalCameraRotation = mainCamera.transform.rotation;
            multiplier += item.ReturnMultiplier();
            finishedMiniGame = false;
            StartCoroutine(MoveCamera(targetCameraPosition.position, targetCameraPosition.rotation, duration));
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
        m_billboard.SetSprite(m_seed.ReturnFinishedSprite());
        StopAllCoroutines();
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public void HarvestCrop()
    {
        state = PlantState.Normal;
        stateText.text = state.ToString();
        image.sprite = null;
        m_billboard.SetSprite(null);
        GameObject particles = Instantiate(m_harvestingParticleSystem);
        particles.transform.SetParent(transform, false);
        particles.transform.position += m_harvestingParticlesOffset;
        tealedGround.SetActive(false);
        untealedGround.SetActive(true);
        
        for (int i = 0; i < multiplier; i++)
        {
            GameObject spawnedItem = objectPool.TakeObjectOut("Crop");
            spawnedItem.transform.position = cropToSpawnLocation.transform.position;
            //GameObject spawnedItem = Instantiate(cropToSpawn, cropToSpawnLocation.transform.position, Quaternion.identity);
        
            spawnedItem.GetComponent<SpawnedItem>().SetItem(m_seed.ReturnCrop());

            // Apply force to throw the item in an arch
            Rigidbody itemRb = spawnedItem.GetComponent<Rigidbody>();
            if (itemRb != null)
            {
                Vector3 throwDirection = CalculateArchVelocity(throwAngle, throwDistance);
                itemRb.velocity = throwDirection;
            }
        }
        
        finishedMiniGame = true;
        StartCoroutine(MoveCamera(originalCameraPosition, originalCameraRotation, duration));
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
    
    private IEnumerator MoveCamera(Vector3 targetPosition, Quaternion targetRotation, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = mainCamera.transform.position;
        Quaternion startRotation = mainCamera.transform.rotation;

        while (elapsedTime < duration)
        {
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            mainCamera.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = targetPosition;
        mainCamera.transform.rotation = targetRotation;
        isCameraInPosition = !isCameraInPosition;
        
        miniGame.SetActive(!miniGame.activeInHierarchy);
        line.SetActive(!line.activeInHierarchy);

        
        playerMovement.enabled = finishedMiniGame; 
    }
}
