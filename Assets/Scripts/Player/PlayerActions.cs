using UnityEngine;
using GabrielBigardi.SpriteAnimator;
using DG.Tweening;

public class PlayerActions : MonoBehaviour
{
    private PlayerInput playerInput;
    private HookMechanic hookMechanic;
    private EatMechanic eatMechanic;

    [Header("Reference")]
    private CooldownManager cooldownManager;
    private PlayerBaseStats playerBaseStats;
    public SpriteAnimator spriteAnimator;
    [SerializeField]private SpriteRenderer playerSpriteRenderer;
    private Color originalPlayerColor = Color.white; // Store the original color

    private void OnEnable()
    {
        HookMechanic.OnHookEnd += HookHitAnimation;
        Enemy.OnPlayerCollide += PlayerHurtSound;
        EatMechanic.onEating += EatAnimation;
    }

    private void OnDisable()
    {
        HookMechanic.OnHookEnd -= HookHitAnimation;
        Enemy.OnPlayerCollide -= PlayerHurtSound;
        EatMechanic.onEating -= EatAnimation;
    }

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        hookMechanic = GetComponentInChildren<HookMechanic>();
        eatMechanic = GetComponentInChildren<EatMechanic>();
        cooldownManager = GetComponent<CooldownManager>();
        playerBaseStats = GetComponent<PlayerBaseStats>();
        
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
