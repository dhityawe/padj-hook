using UnityEngine;
using GabrielBigardi.SpriteAnimator;

public class PlayerActions : MonoBehaviour
{
    private PlayerInput playerInput;
    private HookMechanic hookMechanic;
    private EatMechanic eatMechanic;

    [Header("Reference")]
    private CooldownManager cooldownManager;
    private PlayerBaseStats playerBaseStats;
    public SpriteAnimator spriteAnimator;

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
        spriteAnimator.Play("HookStart").SetOnComplete(() => {
            // Animation completed, now play idle animation
            spriteAnimator.Play("Idle");
        });
        
        HookOutSound();
    }

    private void HookHitAnimation()
    {
        spriteAnimator.Play("HookHit");
        HookHitSound();
    }

    private void EatAnimation()
    {
        spriteAnimator.Play("Eat");
    }

    #endregion

    #region Sound
    private void HookOutSound()
    {
        AudioManager.Instance.PlaySound(0);
    }

    private void HookHitSound()
    {
        AudioManager.Instance.PlaySound(1);
    }

    private void PlayerHurtSound()
    {
        AudioManager.Instance.PlaySound(3);
    }
    #endregion

}
