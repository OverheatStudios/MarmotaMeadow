using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UIElements.Experimental;

public class Plant : MonoBehaviour
{
    private enum PlantState
    {
        Normal,
        Tealed,
        Planted,
        Waterd,
        Completed
    }

    [SerializeField] private SettingsScriptableObject m_settings;
    [SerializeField] private PlantState state = PlantState.Normal;
    [SerializeField] private Seeds m_seed = null;
    [SerializeField] private TextMeshProUGUI growthText;
    [SerializeField] private TextMeshProUGUI m_tillingProgressText;
    [SerializeField] private float growthTimer = 0f;
    [SerializeField] private GameObject m_clockIconGrowthTimer;
    [SerializeField] private float maxGrowthTimer;
    [SerializeField] private float multiplier = 1.0f;
    [SerializeField] private float growOffset = 0;
    [SerializeField] private GameObject cropToSpawn;
    [SerializeField] private float throwAngle = 45f; // Angle of the throw (in degrees)
    [SerializeField] private float throwDistance = 5f;
    [SerializeField] private GameObject cropToSpawnLocation;
    [SerializeField] private float fertilizerMultiplier = 1.5f;
    [SerializeField] private bool planted;
    [SerializeField] private float m_secondsWithoutErrorFeedbackAfterStateChange = 1.5f; // So player doesn't get error sound when spam clicking
    [SerializeField] private BoxCollider extraCollider;
    [SerializeField] private float extraColliderYoffset;
    [SerializeField] private bool m_plantMultiplierIsMultiplicative = true;

    [SerializeField] private Billboard m_billboard;

    [SerializeField] private UnityEvent gamePausedEvent;

    [SerializeField] private MovementScript playerMovement;
    [SerializeField] private Rigidbody playerRb;

    [SerializeField] private ObjectPooling objectPool;
    [SerializeField] private TutorialManager tutorialManager;

    [SerializeField] private GameObject tealedGround;
    [SerializeField] private GameObject untealedGround;

    [SerializeField] private GameObject m_tillingParticleSystem;
    [SerializeField] private Vector3 m_tillingParticlesOffset = new Vector3(0, 0.15f, 0);

    [SerializeField] private GameObject m_harvestingParticleSystem;
    [SerializeField] private Vector3 m_harvestingParticlesOffset = new Vector3(0, 0.15f, 0);

    [Tooltip("Cooldown in seconds before player can interact after an interaction")]
    [SerializeField] private float m_interactCooldown = 0.4f;
    private float m_secondsSinceLastInteraction = 1239048;

    [Header("Sfx")]
    [SerializeField] private AudioClip m_harvestSfx;
    [SerializeField] private AudioClip m_tillSfx;
    [SerializeField] private AudioClip m_waterSfx;
    [SerializeField] private AudioClip m_plantSfx;

    [Header("Tilling Minigame")]
    [SerializeField] private GameObject m_tillSphere;
    [SerializeField] private bool m_randomTillMaskRotation = true;
    [SerializeField] private Collider m_tillCollider;
    [Tooltip("How far can the user till from?")]
    [SerializeField] private float m_maxRayTillDistance = 25;
    [Tooltip("We'll do perAxisChecks^2 checks to determine the tilled percentage, more checks is more accurate but slower")]
    [SerializeField] private int m_numTillCheckPointsPerAxis = 10;
    [Tooltip("How long does the tilling animation last?")]
    [SerializeField] private float m_tillingAnimationDuration = 0.4f;
    [Tooltip("Maximum scale of tilled ground is this value + 1")]
    [SerializeField] private float m_tillingAnimationStrength = 0.8f;
    [Tooltip("What percentage tilled do you need for it to count as tilled? Wouldn't recommend >0.8")]
    [Range(0, 1)][SerializeField] private float m_requiredTillingPercent = 0.5f;

    private readonly List<GameObject> m_tillMasks = new();
    private float m_secondsSinceTilled = -1; // If negative, not tilled
    private Vector3 m_originalTilledGroundScale;
    private float m_currentTilledPercent = 0;

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform targetCameraPosition;
    [SerializeField] private float duration;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    [SerializeField] private bool isCameraInPosition = false;

    [Header("Line MiniGame")]
    [SerializeField] private GameObject m_lineMinigameUi;
    [SerializeField] private TextMeshProUGUI m_lineMinigameExitUi;
    [SerializeField] private GameObject harvestingMiniGame;
    [SerializeField] private GameObject wateringMinigame;
    [SerializeField] private GameObject line;
    [SerializeField] private bool finishedMiniGame = true;
    [SerializeField] private bool inHarvestingMiniGame = false;
    [SerializeField] private bool inWateringMiniGame = false;
    [SerializeField] private float toolMultiplier;
    [SerializeField] private GameObject m_cropUiCanvas;

    [Header("Tooltips")]
    [SerializeField] private Vector3 m_actionTooltipOffset = Vector3.up * 0.4f;
    [SerializeField] private GameObject m_goodTooltip;

    public System.Action OnTealed;
    public System.Action OnPlanted;
    public System.Action OnWatered;
    public System.Action OnHarvested;
    private float m_secondsSinceStateChange = 0;
    private PlantState m_stateLastFrame;

    // Start is called before the first frame update
    void Start()
    {
        m_lineMinigameUi.SetActive(false);
        m_originalTilledGroundScale = tealedGround.transform.localScale;
        growthTimer = maxGrowthTimer;
        m_tillingProgressText.gameObject.SetActive(state == PlantState.Normal);
        m_billboard.SetSprite(null);
        tealedGround.SetActive(true);
        untealedGround.SetActive(true);
        m_clockIconGrowthTimer.SetActive(false);
        growthText.gameObject.SetActive(false);
        m_stateLastFrame = state;
        if (SceneManager.GetActiveScene().name == "NightScene")
        {
            return;
        }
        objectPool = GameObject.FindGameObjectWithTag("ObjectPool").GetComponent<ObjectPooling>();
        tutorialManager = GameObject.FindGameObjectWithTag("TutorialManager").GetComponent<TutorialManager>();
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<MovementScript>();
        playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "NightScene")
        {
            m_cropUiCanvas.SetActive(false);
            m_lineMinigameExitUi.gameObject.SetActive(false);
            m_lineMinigameUi.gameObject.SetActive(false);
            return;
        }
        m_cropUiCanvas.SetActive(!m_lineMinigameExitUi.isActiveAndEnabled); // Hides "requires watering" and "ready" text during line minigames

        // How long since the plant state changed?
        m_secondsSinceStateChange += Time.deltaTime;
        if (m_stateLastFrame != state)
        {
            m_stateLastFrame = state;
            m_secondsSinceStateChange = 0;
            Instantiate(m_goodTooltip).transform.position = transform.position + m_actionTooltipOffset;
        }

        // Tilling minigame
        m_secondsSinceLastInteraction += Time.deltaTime;
        m_tillingProgressText.text = "Tilling Progress: " + ((int)(Mathf.InverseLerp(0, m_requiredTillingPercent, m_currentTilledPercent) * 100)) + "%";
        m_tillingProgressText.gameObject.SetActive(state == PlantState.Tealed || state == PlantState.Normal);

        // UI
        m_clockIconGrowthTimer.SetActive(state == PlantState.Waterd);
        growthText.gameObject.SetActive(state == PlantState.Waterd || state == PlantState.Completed || state == PlantState.Planted);
        m_lineMinigameExitUi.text = m_lineMinigameExitUi.text.Replace("[KEY]", GameInput.GetKeybind("ExitMinigame").ToString());
        HandleTillingAnimation();

        if (growthTimer < maxGrowthTimer / 2 && state == PlantState.Waterd)
        {
            m_billboard.SetSprite(m_seed.ReturnGrowingSprite());
        }

        // Exit line minigame
        if (isCameraInPosition && GameInput.GetKeybind("ExitMinigame").GetKeyDown()) // Right mouse button to reset camera
        {
            finishedMiniGame = true;
            StartCoroutine(MoveCamera(originalCameraPosition, originalCameraRotation, duration, false, false));

            if (!planted)
            {
                planted = true;
            }
        }

        // Grow the crop
        if (planted && state == PlantState.Waterd)
        {

            growthTimer -= Time.deltaTime;
            growthText.text = ((int)growthTimer).ToString();

            if (growthTimer <= 0)
            {
                growthText.text = "Ready";
                planted = false;
                growthTimer = maxGrowthTimer;
                state = PlantState.Completed;
                m_billboard.SetSprite(m_seed.ReturnFinishedSprite());
                extraCollider.size = new Vector3(extraCollider.size.x, extraCollider.size.y + extraColliderYoffset, extraCollider.size.z);
            }
        }
    }

    private void HandleTillingAnimation()
    {
        if (m_secondsSinceTilled < 0) return;

        m_secondsSinceTilled += Time.deltaTime;
        if (m_tillingAnimationDuration <= m_secondsSinceTilled)
        {
            tealedGround.transform.localScale = m_originalTilledGroundScale;
            return;
        }

        float t = Mathf.InverseLerp(0, m_tillingAnimationDuration, m_secondsSinceTilled) * 2;
        if (t > 1) t = 2 - t; // Second half, going back to normal

        t = Easing.InOutBounce(t);
        tealedGround.transform.localScale = m_originalTilledGroundScale * (1 + m_tillingAnimationStrength * t);
    }

    private void CreateTillMask(Vector3 worldPos)
    {
        GameObject obj = Instantiate(m_tillSphere);
        obj.transform.position = worldPos + Vector3.down * (obj.GetComponent<Renderer>().bounds.size.y / 2.0f);

        if (m_randomTillMaskRotation)
        {
            obj.transform.rotation *= Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
        }

        Mask.AddScriptToObject(obj, new GameObject[1] { untealedGround });

        m_tillMasks.Add(obj);
    }

    private void OnHoe(InventoryItem item)
    {
        RaycastHit hit;
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        if (!m_tillCollider.Raycast(ray, out hit, m_maxRayTillDistance)) return;

        CreateTillMask(hit.point);

        GameObject particles = Instantiate(m_tillingParticleSystem);
        particles.transform.position = hit.point + m_tillingParticlesOffset;

        AudioSource.PlayClipAtPoint(m_tillSfx, transform.position, m_settings.GetSettings().GetGameVolume());

        m_currentTilledPercent = GetTilledPercent();
        if (m_currentTilledPercent >= m_requiredTillingPercent)
        {
            HoeFinish(item);
        }
    }

    /// <summary>
    /// Check how much of the ground is tilled, this function is a little costly so don't run it too often.
    /// Note that getting this to 100% tilled is quite hard, especially considering you till in circles.
    /// </summary>
    /// <returns>Tilled percent between 0 and 1</returns>
    private float GetTilledPercent()
    {
        var bounds = untealedGround.GetComponent<Renderer>().bounds;

        float checkDistanceX = (bounds.max.x - bounds.min.x) / (float)m_numTillCheckPointsPerAxis;
        float checkDistanceZ = (bounds.max.z - bounds.min.z) / (float)m_numTillCheckPointsPerAxis;

        float totalPercent = 0;

        for (int x = 0; x < m_numTillCheckPointsPerAxis; x++)
        {
            for (int z = 0; z < m_numTillCheckPointsPerAxis; z++)
            {
                Vector3 point = new Vector3(
                    Mathf.Lerp(bounds.min.x, bounds.max.x, x / (float)m_numTillCheckPointsPerAxis),
                    0,
                    Mathf.Lerp(bounds.min.z, bounds.max.z, z / (float)m_numTillCheckPointsPerAxis)
                );

                float closestX = float.MaxValue;
                float closestZ = float.MaxValue;
                foreach (GameObject obj in m_tillMasks)
                {
                    float xDistance = Mathf.Abs(obj.transform.position.x - point.x);
                    float zDistance = Mathf.Abs(obj.transform.position.z - point.z);

                    if (xDistance < closestX) closestX = xDistance;
                    if (zDistance < closestZ) closestZ = zDistance;
                }

                float thisPercent = 0.5f * Mathf.InverseLerp(checkDistanceX, 0, closestX) + 0.5f * Mathf.InverseLerp(checkDistanceZ, 0, closestZ);
                totalPercent += 1.0f / (float)(m_numTillCheckPointsPerAxis * m_numTillCheckPointsPerAxis) * thisPercent;
            }
        }

        return totalPercent;
    }

    private void HoeFinish(InventoryItem item)
    {
        foreach (var obj in m_tillMasks)
        {
            Destroy(obj);
        }
        m_tillMasks.Clear();
        untealedGround.SetActive(false);
        tealedGround.SetActive(true);

        state = PlantState.Tealed;
        OnTealed?.Invoke();
        multiplier += item.ReturnMultiplier();
        tealedGround.SetActive(true);
        untealedGround.SetActive(false);

        m_originalTilledGroundScale = untealedGround.transform.localScale;
        m_secondsSinceTilled = 0;
    }

    public bool CanGiveErrorFeedback()
    {
        return m_secondsSinceStateChange >= m_secondsWithoutErrorFeedbackAfterStateChange;
    }

    public bool ChangeState(InventoryItem item)
    {
        if (m_secondsSinceLastInteraction < m_interactCooldown) return false;

        if (item.item.name == "Fertilizer" && state != PlantState.Completed)
        {
            multiplier += fertilizerMultiplier;
            return true;
        }
        else if (item.item.name == "hoe" && state == PlantState.Normal)
        {
            OnHoe(item);
            return true;
        }
        else if (item.item is Seeds seeds && state == PlantState.Tealed)
        {
            AudioSource.PlayClipAtPoint(m_plantSfx, transform.position, m_settings.GetSettings().GetGameVolume());
            // reset hoe percent
            m_currentTilledPercent = 0;
            //changing state
            state = PlantState.Planted;
            //adding the seed
            m_seed = seeds;
            growthTimer = m_seed.ReturnGrowDuration() + growOffset;
            maxGrowthTimer = m_seed.ReturnGrowDuration() + growOffset;
            planted = true;
            if (m_plantMultiplierIsMultiplicative) multiplier = (multiplier < 1 ? 1 : multiplier) * m_seed.ReturnAmount();
            else multiplier += m_seed.ReturnAmount();
            //some visual feedback
            m_billboard.SetSprite(m_seed.ReturnPlantedSprite());
            OnPlanted?.Invoke();
            growthText.text = "Requires Watering";
            return true;
        }
        else if (item.item.name == "watering can" && state == PlantState.Planted && planted && !inWateringMiniGame)
        {
            inWateringMiniGame = true;
            playerMovement.enabled = false;
            playerRb.velocity = Vector2.zero;
            originalCameraPosition = mainCamera.transform.position;
            originalCameraRotation = mainCamera.transform.rotation;
            finishedMiniGame = false;
            toolMultiplier = item.ReturnMultiplier();
            StartCoroutine(MoveCamera(targetCameraPosition.position, targetCameraPosition.rotation, duration, false, true));
            return true;
        }
        else if (item.item.name == "harvesting tool" && state == PlantState.Completed && !inHarvestingMiniGame)
        {
            inHarvestingMiniGame = true;
            playerMovement.enabled = false;
            playerRb.velocity = Vector2.zero;
            originalCameraPosition = mainCamera.transform.position;
            originalCameraRotation = mainCamera.transform.rotation;
            finishedMiniGame = false;
            toolMultiplier = item.ReturnMultiplier();
            StartCoroutine(MoveCamera(targetCameraPosition.position, targetCameraPosition.rotation, duration, true, false));
            return true;
        }
        return false;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void HarvestCrop()
    {
        m_lineMinigameUi.SetActive(false);

        state = PlantState.Normal;
        m_billboard.SetSprite(null);
        GameObject particles = Instantiate(m_harvestingParticleSystem);
        particles.transform.SetParent(transform, false);
        particles.transform.position += m_harvestingParticlesOffset;
        tealedGround.SetActive(true);
        untealedGround.SetActive(true);
        m_secondsSinceTilled = -1;

        multiplier += toolMultiplier;
        toolMultiplier = 0;

        AudioSource.PlayClipAtPoint(m_harvestSfx, transform.position, m_settings.GetSettings().GetGameVolume());

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
                Vector3 throwDirection = CalculateArchVelocity();
                itemRb.velocity = throwDirection;
            }
        }

        multiplier = 0;

        finishedMiniGame = true;
        OnHarvested?.Invoke();
        extraCollider.size = new Vector3(extraCollider.size.x, 1, extraCollider.size.z);
        StartCoroutine(MoveCamera(originalCameraPosition, originalCameraRotation, duration, false, false));
    }

    Vector3 CalculateArchVelocity()
    {
        float minAngle = 10;
        float maxAngle = 60;
        float angleRad = UnityEngine.Random.Range(minAngle, maxAngle) * Mathf.Deg2Rad;

        float min = 1.5f;
        float max = 7.0f;
        float distance = UnityEngine.Random.Range(min, max);

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

    private IEnumerator MoveCamera(Vector3 targetPosition, Quaternion targetRotation, float duration, bool inHarvestMiniGame, bool inWaterMiniGame)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = mainCamera.transform.position;
        Quaternion startRotation = mainCamera.transform.rotation;

        planted = !planted;

        inWateringMiniGame = inWaterMiniGame;
        inHarvestingMiniGame = inHarvestMiniGame;

        m_lineMinigameUi.SetActive(inHarvestMiniGame || inWaterMiniGame);
        harvestingMiniGame.SetActive(inHarvestMiniGame);
        wateringMinigame.SetActive(inWaterMiniGame);

        while (elapsedTime < duration)
        {
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            mainCamera.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        
        line.SetActive(!line.activeInHierarchy);

        mainCamera.transform.position = targetPosition;
        mainCamera.transform.rotation = targetRotation;
        isCameraInPosition = !isCameraInPosition;

        playerMovement.enabled = finishedMiniGame;
    }

    public void IncreaseMultiplier(float amount)
    {
        multiplier = amount;
    }

    public void WaterCrop()
    {
        m_lineMinigameUi.SetActive(false);
        finishedMiniGame = true;
        state = PlantState.Waterd;
        OnWatered?.Invoke();
        AudioSource.PlayClipAtPoint(m_waterSfx, transform.position, m_settings.GetSettings().GetGameVolume());
        multiplier += toolMultiplier;
        toolMultiplier = 0;
        StartCoroutine(MoveCamera(originalCameraPosition, originalCameraRotation, duration, false, false));
    }

    public void SetGrowTimerOffset(float offset)
    {
        growOffset += offset;
    }

    public bool IsPreTilledState()
    {
        return state == PlantState.Normal;
    }
}
