using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIController : MonoBehaviour
{
    [Header("Hotkey UI References")]
    [SerializeField] private Image hookHotkeyImage; // Assign the Q key UI image
    [SerializeField] private Image eatHotkeyImage;  // Assign the Space key UI image
    
    [Header("Visual Settings")]
    [SerializeField] private float scaleDownAmount = 0.9f; // How much to scale down (0.9 = 90% of original size)
    [SerializeField] private float animationDuration = 0.05f; // Snappy animation for 8-bit feel
    [SerializeField] private Color pressedColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Dark gray instead of pure black
    
    private Color originalHookColor;
    private Color originalEatColor;
    private Vector3 originalHookScale;
    private Vector3 originalEatScale;
    
    void Start()
    {
        // Store original colors and scales
        if (hookHotkeyImage != null)
        {
            originalHookColor = hookHotkeyImage.color;
            originalHookScale = hookHotkeyImage.transform.localScale;
        }
        
        if (eatHotkeyImage != null)
        {
            originalEatColor = eatHotkeyImage.color;
            originalEatScale = eatHotkeyImage.transform.localScale;
        }
    }
    
    void Update()
    {
        HandleHookInput();
        HandleEatInput();
    }
    
    private void HandleHookInput()
    {
        if (hookHotkeyImage == null) return;
        
        // Q key down - make black and scale down
        if (Input.GetKeyDown(KeyCode.Q))
        {
            HookKeyPressed();
        }
        // Q key up - return to normal
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            HookKeyReleased();
        }
    }
    
    private void HandleEatInput()
    {
        if (eatHotkeyImage == null) return;
        
        // Space key down - make black and scale down
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EatKeyPressed();
        }
        // Space key up - return to normal
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            EatKeyReleased();
        }
    }
    
    private void HookKeyPressed()
    {
        // Kill any existing tweens to prevent conflicts
        hookHotkeyImage.DOKill();
        hookHotkeyImage.transform.DOKill();
        
        // Change color to dark gray and scale down with snappy animation
        hookHotkeyImage.DOColor(pressedColor, animationDuration).SetEase(Ease.Linear);
        hookHotkeyImage.transform.DOScale(originalHookScale * scaleDownAmount, animationDuration)
            .SetEase(Ease.Linear);
    }
    
    private void HookKeyReleased()
    {
        // Kill any existing tweens to prevent conflicts
        hookHotkeyImage.DOKill();
        hookHotkeyImage.transform.DOKill();
        
        // Return to original color and scale with snappy animation
        hookHotkeyImage.DOColor(originalHookColor, animationDuration).SetEase(Ease.Linear);
        hookHotkeyImage.transform.DOScale(originalHookScale, animationDuration)
            .SetEase(Ease.Linear);
    }
    
    private void EatKeyPressed()
    {
        // Kill any existing tweens to prevent conflicts
        eatHotkeyImage.DOKill();
        eatHotkeyImage.transform.DOKill();
        
        // Change color to dark gray and scale down with snappy animation
        eatHotkeyImage.DOColor(pressedColor, animationDuration).SetEase(Ease.Linear);
        eatHotkeyImage.transform.DOScale(originalEatScale * scaleDownAmount, animationDuration)
            .SetEase(Ease.Linear);
    }
    
    private void EatKeyReleased()
    {
        // Kill any existing tweens to prevent conflicts
        eatHotkeyImage.DOKill();
        eatHotkeyImage.transform.DOKill();
        
        // Return to original color and scale with snappy animation
        eatHotkeyImage.DOColor(originalEatColor, animationDuration).SetEase(Ease.Linear);
        eatHotkeyImage.transform.DOScale(originalEatScale, animationDuration)
            .SetEase(Ease.Linear);
    }
}
