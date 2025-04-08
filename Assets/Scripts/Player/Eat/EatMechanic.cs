using UnityEngine;
using System.Collections;
using GabrielBigardi.SpriteAnimator;

public class EatMechanic : MonoBehaviour
{
    private IEatDataProvider eatDataProvider;
    private Collider2D eatCollider;
    private bool isEating = false;

    [Header("Reference")]
    public SpriteAnimator eatAnimator;

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
        eatAnimator.Play("Biting"); // Play eat animation
        isEating = true;
        StartCoroutine(EatRoutine());
    }

    private IEnumerator EatRoutine()
    {
        eatCollider.enabled = true; // Enable eat detection
        yield return new WaitForSeconds(0.2f); // Small window to detect collision
        eatCollider.enabled = false;

        yield return new WaitForSeconds(eatDataProvider.EatCooldown); // Cooldown before next eat
        isEating = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.EnemyKill(); // Kill enemy
                Debug.Log("You ate an enemy!");
            }
        }
    }
}
