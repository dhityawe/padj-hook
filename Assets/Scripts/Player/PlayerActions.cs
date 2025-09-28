using UnityEngine;
using GabrielBigardi.SpriteAnimator;
using DG.Tweening;
using System;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private HookMechanic hookMechanic;
    [SerializeField] private EatMechanic eatMechanic;

    [Header("Reference")]
    [SerializeField] private CooldownManager cooldownManager;
    [SerializeField] private PlayerBaseStats playerBaseStats;
    [SerializeField] public SpriteAnimator spriteAnimator;
    [SerializeField]private SpriteRenderer playerSpriteRenderer;
    private Color originalPlayerColor = Color.white; // Store the original color
    private float originalCameraSize; // Store the original camera size

    private void OnEnable()
    {
        HookMechanic.OnHookEnd += HookHitAnimation;
        Enemy.OnPlayerCollide += PlayerHurtSound;
        Enemy.OnEated += EatVisualEffects;
        EatMechanic.onEating += EatAnimation;
    }

    private void OnDisable()
    {
        HookMechanic.OnHookEnd -= HookHitAnimation;
        Enemy.OnPlayerCollide -= PlayerHurtSound;
        Enemy.OnEated -= EatVisualEffects;
        EatMechanic.onEating -= EatAnimation;
    }

    void Start()
    {       
        // Get the SpriteRenderer from the SpriteAnimator for hit effects
        if (spriteAnimator != null)
        {
            playerSpriteRenderer = spriteAnimator.GetComponent<SpriteRenderer>();
            
            // If not found on the SpriteAnimator, try to find it in children
            if (playerSpriteRenderer == null)
            {
                playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
            
            // Final check - log if still not found
            if (playerSpriteRenderer == null)
            {
                Debug.LogWarning("SpriteRenderer not found for PlayerHitAnimation. Hit effect will be disabled.");
            }
            else
            {
                // Store the original color when we first find the SpriteRenderer
                originalPlayerColor = playerSpriteRenderer.color;
            }
        }

        hookMechanic.Initialize(playerBaseStats); // Ensure HookMechanic gets the required data

        // Store the original camera size at start
        if (Camera.main != null)
        {
            originalCameraSize = Camera.main.orthographicSize;
        }

        playerInput.OnHookPressed += HookEntity;
        playerInput.OnEatPressed += EatEntity;
    }

    void HookEntity()
    {
        Debug.Log("Hooking! on PlayerActions");
        cooldownManager.StartCooldown("Hook");
        hookMechanic?.HookEntity();
        HookAnimation();
    }

    void EatEntity()
    {
        if (cooldownManager.IsOnCooldown("Eat"))
        {
            Debug.Log("Eat is on cooldown!");
            return;
        }

        cooldownManager.StartCooldown("Eat");
        eatMechanic?.EatEntity();
    }



    #region Animation
    private void HookAnimation()
    {
        // Wait for animation to end naturally, then play idle
        spriteAnimator.Play("HookStart");
        HookOutSound();
    }

    private void HookHitAnimation()
    {
        spriteAnimator.Play("HookHit").SetOnComplete(() => {
            spriteAnimator.Play("Idle");
        });

        HookHitSound();
    }

    private void EatAnimation()
    {
        spriteAnimator.Play("Eat").SetOnComplete(() => {
            spriteAnimator.Play("Idle");
        });
    }
    
    private void EatVisualEffects()
    {
        // Kill any existing tweens to prevent interference
        transform.DOKill();
        playerSpriteRenderer?.DOKill();
        
        // Scale pulse effect - player grows briefly then returns to normal
        Vector3 originalScale = transform.localScale;
        transform.DOScale(originalScale * 1.2f, 0.1f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => {
                transform.DOScale(originalScale, 0.15f)
                    .SetEase(Ease.InOutQuad);
            });
        
        // Improved screen zoom effect with better synchronization
        if (Camera.main != null)
        {
            // Kill any existing camera tweens completely to prevent stacking
            Camera.main.DOKill(true); // Complete kill including callbacks
            
            // Always reset to original size first to ensure consistency
            Camera.main.orthographicSize = originalCameraSize;
            
            // Create a single, atomic zoom sequence that can't be interrupted
            Sequence zoomSequence = DOTween.Sequence();
            zoomSequence.Append(Camera.main.DOOrthoSize(originalCameraSize * 0.95f, 0.025f).SetEase(Ease.OutQuad))
                       .Append(Camera.main.DOOrthoSize(originalCameraSize, 0.15f).SetEase(Ease.InOutQuad))
                       .OnComplete(() => {
                           // Ensure we're exactly at original size when done
                           if (Camera.main != null)
                           {
                               Camera.main.orthographicSize = originalCameraSize;
                           }
                       });
        }
    }

    private void PlayerHitAnimation()
    {
        Debug.Log("Player hit animation triggered");
        
        // Check if SpriteRenderer is available before using it
        if (playerSpriteRenderer == null)
        {
            Debug.LogWarning("PlayerSpriteRenderer is null, cannot play hit animation");
            return;
        }
        
        // Kill any existing color tweens to prevent interference from multiple hits
        playerSpriteRenderer.DOKill();
        
        // Kill any existing shake tweens on this transform
        transform.DOKill();
        
        // Add shake effect to the player
        transform.DOShakePosition(0.2f, 0.1f, 10, 90, false, true)
            .SetEase(Ease.OutQuad);
        
        // Create a hit effect: flash to black and back to original color
        playerSpriteRenderer.DOColor(Color.black, 0.1f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => {
                // Return to original color
                if (playerSpriteRenderer != null) // Additional safety check
                {
                    playerSpriteRenderer.DOColor(originalPlayerColor, 0.1f)
                        .SetEase(Ease.InQuad);
                }
            });
    }

    #endregion

    #region Sound
    private void HookOutSound()
    {
        AudioManager.Instance.PlaySound(0, 0.3f);
    }

    private void HookHitSound()
    {
        AudioManager.Instance.PlaySound(1, 0.3f);
    }

    private void PlayerHurtSound()
    {
        AudioManager.Instance.PlaySound(3, 0.8f);
        PlayerHitAnimation(); // Play hit effect when player gets hurt
        Debug.Log("Player hurt!");
    }
    #endregion

}
