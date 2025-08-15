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
    }

    private void OnDisable()
    {
        HookMechanic.OnHookEnd -= HookHitAnimation;
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
        EatAnimation();
    }


    
    #region Animation
    private void HookAnimation()
    {
        spriteAnimator.Play("HookStart");
    }

    private void HookHitAnimation()
    {
        spriteAnimator.Play("HookHit");
    }

    private void EatAnimation()
    {
        spriteAnimator.Play("Eat");
    }
    #endregion
}
