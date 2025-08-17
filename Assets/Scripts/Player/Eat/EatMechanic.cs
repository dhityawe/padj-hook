using UnityEngine;
using System.Collections;
using GabrielBigardi.SpriteAnimator;
using System;

public class EatMechanic : MonoBehaviour
{
    private IEatDataProvider eatDataProvider;
    private Collider2D eatCollider;
    private bool isEating = false;

    [Header("Reference")]
    public SpriteAnimator eatAnimator;
    public static Action onEating;

    private void Start()
    {
        eatDataProvider = GetComponentInParent<IEatDataProvider>(); // Get stats from PlayerBaseStats
        eatCollider = GetComponent<Collider2D>();

        PlayerInput playerInput = GetComponentInParent<PlayerInput>(); // Get input reference
        if (playerInput != null)
        {
            playerInput.OnEatPressed += EatEntity; // Subscribe to Eat event
        }

        eatCollider.enabled = false; // Disable collider initially
    }

    public void EatEntity()
    {
        if (isEating) return;
        onEating?.Invoke();
        eatAnimator.Play("Biting"); // Play eat animation
        EatSound();
        isEating = true;
        StartCoroutine(EatRoutine());
    }

    private IEnumerator EatRoutine()
    {   
        StartCoroutine(ColliderRoutine()); // Start collider routine
        yield return new WaitForSeconds(eatDataProvider.EatCooldown); // Cooldown before next eat
        isEating = false;
    }

    private IEnumerator ColliderRoutine()
    {
        eatCollider.enabled = true; // Enable eat detection
        yield return new WaitForSeconds(0.2f); // Small window to detect collision
        eatCollider.enabled = false;
    }

    private void EatSound()
    {
        AudioManager.Instance.PlaySound(2, 0.6f); // Play eating sound
    }
}
