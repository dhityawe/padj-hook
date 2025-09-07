using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider hookCooldownSlider;
    [SerializeField] private Slider eatCooldownSlider;
    
    [Header("Screen Shake")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float shakeIntensity = 0.5f;
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private int shakeVibrato = 10;
    private bool isShaking = false;

    private void OnEnable()
    {   
        Enemy.OnPlayerCollide += ScreenShake;
        // Ensure CooldownManager is initialized before subscribing
        if (CooldownManager.Instance != null)
        {
            CooldownManager.Instance.OnCooldownStart += HandleCooldownStart;
        }
        else
        {
            Debug.LogError("CooldownManager is not initialized yet!");
        }
    }


    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        Enemy.OnPlayerCollide -= ScreenShake;
        CooldownManager.Instance.OnCooldownStart -= HandleCooldownStart;
    }

    private void Start()
    {
        // Initialize cooldown sliders to 0 (ready state)
        hookCooldownSlider.value = 0;
        eatCooldownSlider.value = 0;
        
        // Auto-assign main camera if not set
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        if (mainCamera == null)
        {
            Debug.LogWarning("Main camera not found for screen shake functionality!");
        }
    }

    private void HandleCooldownStart(string skillName, float currentTime, float duration)
    {
        if (skillName == "Hook")
        {        
            hookCooldownSlider.value = Mathf.Lerp(0, 1, currentTime / duration);
        }

        else if (skillName == "Eat")
        {
            eatCooldownSlider.value = Mathf.Lerp(0, 1, currentTime / duration);
        }
    }

    private IEnumerator UpdateCooldown(Slider cooldownSlider, float cooldownTime)
    {
        cooldownSlider.value = 1;
        float currentTime = cooldownTime;

        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            cooldownSlider.value = Mathf.Lerp(0, 1, currentTime / cooldownTime);
            //Debug.Log(Mathf.Lerp(0, 1, currentTime / cooldownTime));
            yield return null;
        }

        cooldownSlider.value = 0;
    }
    
    #region Screen Shake
    public void ScreenShake()
    {
        // Prevent multiple shakes from stacking
        if (isShaking) return;
        
        ScreenShake(shakeIntensity, shakeDuration, shakeVibrato);
    }
    
    public void ScreenShake(float intensity)
    {
        // Prevent multiple shakes from stacking
        if (isShaking) return;
        
        ScreenShake(intensity, shakeDuration, shakeVibrato);
    }
    
    public void ScreenShake(float intensity, float duration, int vibrato)
    {
        if (mainCamera == null)
        {
            Debug.LogWarning("Cannot perform screen shake - main camera is null!");
            return;
        }
        
        // Prevent multiple shakes from stacking
        if (isShaking) return;
        
        isShaking = true;
        
        // Kill any existing shake to prevent conflicts
        mainCamera.transform.DOKill();
        
        // Perform the screen shake
        mainCamera.transform.DOShakePosition(duration, intensity, vibrato, 90, false, true)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => {
                isShaking = false; // Reset flag when shake completes
            });
    }
    #endregion
}
