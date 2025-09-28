using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using GabrielBigardi.SpriteAnimator;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public static Action onFever;
    public static Action onFeverEnd;
    public static bool IsFeverActive { get; private set; } = false;
    
    [SerializeField]
    private List<Transform> rightWaypoints = new List<Transform>();

    [SerializeField]
    private List<Transform> leftWaypoints = new List<Transform>();

    [SerializeField]
    [Range(0, 100)]
    private float spawnRate = 50f;

    [SerializeField] private float feverTimeCooldown = 20f;
    [SerializeField] private float feverTimeDuration = 10f;

    [Header("Fever Visuals Control")]
    [SerializeField] private GameObject globalLight;
    [SerializeField] private List<GameObject> lightings;
    [SerializeField] private List<SpriteAnimator> campfireAnimators;
    [SerializeField] private GameObject killStreakEffect;

    private float spawnTimer;
    private bool isFeverActive = false;
    private float feverTimer = 0f;
    private float feverCooldownTimer = 0f;
    private float originalSpawnRate;
    private bool hasTriggeredFeverEnd = false; // Track if fever end has been triggered
    
    // Lighting effect variables
    private Coroutine lightingCoroutine;

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (!isFeverActive)
        {
            feverCooldownTimer += Time.deltaTime;
            if (feverCooldownTimer >= feverTimeCooldown)
            {
                ActivateFever();
                feverCooldownTimer = 0f;
            }
        }
        else
        {
            feverTimer += Time.deltaTime;
            
            // Fire onFeverEnd event 1 second before fever actually ends
            if (feverTimer >= (feverTimeDuration - 1f) && !hasTriggeredFeverEnd)
            {
                onFeverEnd?.Invoke();
                hasTriggeredFeverEnd = true;
                Debug.Log("Fever End effects triggered - 1 second before fever deactivation");
            }
            
            if (feverTimer >= feverTimeDuration)
            {
                DeactivateFever();
            }
        }

        if (spawnTimer >= 1f)
        {
            spawnTimer = 0f;
            if (Random.Range(0, 100) < spawnRate)
            {
                SpawnEnemy();
            }
        }
    }
    private void ActivateFever()
    {
        isFeverActive = true;
        IsFeverActive = true; // Update static property
        feverTimer = 0f;
        hasTriggeredFeverEnd = false; // Reset the fever end trigger flag
        originalSpawnRate = spawnRate;
        spawnRate *= 2f;
        
        // Smoothly fade global light from 1 to 0
        if (globalLight != null)
        {
            Light2D globalLight2D = globalLight.GetComponent<Light2D>();
            if (globalLight2D != null)
            {
                globalLight2D.DOKill(); // Kill any existing tweens
                DOTween.To(() => globalLight2D.intensity, x => globalLight2D.intensity = x, 0f, 1f)
                    .SetEase(Ease.InOutQuad);
            }
        }
        
        // Start campfire lighting effect
        if (lightingCoroutine != null)
        {
            StopCoroutine(lightingCoroutine);
        }
        lightingCoroutine = StartCoroutine(CampfireLightingEffect());
        campfireAnimators.ForEach(animator => animator.Play("Burn"));
        
        // Smoothly fade in killStreak effect UI from 0 to 255 opacity
        if (killStreakEffect != null)
        {
            Image killStreakImage = killStreakEffect.GetComponent<Image>();
            if (killStreakImage != null)
            {
                killStreakImage.DOKill(); // Kill any existing tweens
                killStreakImage.DOFade(1f, 1f).SetEase(Ease.InOutQuad); // 1f = full opacity (255/255)
            }
        }
        
        // Invoke fever event for other scripts to respond
        onFever?.Invoke();
        
        Debug.Log("Fever Time Activated! Spawn rate increased.");
    }

    private void DeactivateFever()
    {
        isFeverActive = false;
        IsFeverActive = false; // Update static property
        spawnRate = originalSpawnRate;
        
        // Smoothly fade global light from 0 back to 1
        if (globalLight != null)
        {
            Light2D globalLight2D = globalLight.GetComponent<Light2D>();
            if (globalLight2D != null)
            {
                globalLight2D.DOKill(); // Kill any existing tweens
                DOTween.To(() => globalLight2D.intensity, x => globalLight2D.intensity = x, 1f, 1f)
                    .SetEase(Ease.InOutQuad);
            }
        }
        
        // Stop campfire lighting effect and smoothly reset lighting to normal
        if (lightingCoroutine != null)
        {
            campfireAnimators.ForEach(animator => animator.Play("Idle"));
            StopCoroutine(lightingCoroutine);
            lightingCoroutine = null;
        }
        
        // Smoothly fade all campfire lights back to 0 intensity
        foreach (GameObject lightObj in lightings)
        {
            if (lightObj != null)
            {
                Light2D light2D = lightObj.GetComponent<Light2D>();
                if (light2D != null)
                {
                    light2D.DOKill(); // Kill any existing tweens
                    DOTween.To(() => light2D.intensity, x => light2D.intensity = x, 0f, 1f)
                        .SetEase(Ease.InOutQuad);
                }
            }
        }
        
        // Smoothly fade out killStreak effect UI from 255 to 0 opacity
        if (killStreakEffect != null)
        {
            Image killStreakImage = killStreakEffect.GetComponent<Image>();
            if (killStreakImage != null)
            {
                killStreakImage.DOKill(); // Kill any existing tweens
                killStreakImage.DOFade(0f, 1f).SetEase(Ease.InOutQuad); // 0f = transparent (0/255)
            }
        }
        
        // Invoke fever end event for enemies to revert animations
        onFeverEnd?.Invoke();
        
        Debug.Log("Fever Time Deactivated. Spawn rate normalized.");
    }

    private void SpawnEnemy()
    {
        int randomIndex = Random.Range(0, 2);
        Transform spawnPoint = randomIndex == 0 ? rightWaypoints[Random.Range(0, rightWaypoints.Count)] : leftWaypoints[Random.Range(0, leftWaypoints.Count)];
        Transform waypoint = randomIndex == 0 ? leftWaypoints[Random.Range(0, leftWaypoints.Count)] : rightWaypoints[Random.Range(0, rightWaypoints.Count)];

        Enemy enemy = EnemyPool.SpawnEnemy();
        enemy.SetSpeed(Random.Range(1f, 3f));
        enemy.transform.position = spawnPoint.position;
        enemy.GetWaypoint().position = waypoint.position;
        
        enemy.SetWaypointParent(waypoint);
        enemy.transform.parent = transform;
    }
    
    private IEnumerator CampfireLightingEffect()
    {
        while (isFeverActive)
        {
            // Create flickering effect for each light
            foreach (GameObject lightObj in lightings)
            {
                if (lightObj != null)
                {
                    Light2D light2D = lightObj.GetComponent<Light2D>();
                    if (light2D != null)
                    {
                        // Random intensity between 0.5 and 1.0 with some flickering
                        float baseIntensity = Mathf.Lerp(0.5f, 1f, (Mathf.Sin(Time.time * Random.Range(3f, 8f)) + 1f) / 2f);
                        
                        // Add some random flickering for more realistic campfire effect
                        float flicker = Random.Range(-0.1f, 0.1f);
                        light2D.intensity = Mathf.Clamp(baseIntensity + flicker, 0.4f, 1.1f);
                    }
                }
            }
            
            // Update lighting effect multiple times per second for smooth flickering
            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
        }
    }
}
