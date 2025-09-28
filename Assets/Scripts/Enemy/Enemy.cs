using System;
using UnityEngine;
using GabrielBigardi.SpriteAnimator;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    public static Action OnPlayerCollide;
    public static Action OnEated;
    public static Action OnCursedEffect;
    public static Action AddScore;
    public SpriteAnimator spriteAnimator;

    [Header("Fever Sprite Ref")]
    public SpriteAnimationObject currentAnimData;
    public SpriteAnimationObject feverAnimData;

    private BoxCollider2D boxCollider;

    [SerializeField]
    private Transform waypoint;

    [SerializeField]
    private float speed = 1f;

    private Transform hook;

    private Vector2 playerPosition;

    private bool isHooked = false;
    private bool soundPlayed = false;
    private bool isDying = false; // Track if enemy is dying to prevent animation conflicts
    private bool hookedAnimationPlayed = false; // Track if hooked animation was already played
    private bool facingRight = true; // Track which direction the sprite is facing
    private bool hasInitializedFacing = false; // Track if we've set the initial facing direction
    
    // Fever RGB animation variables
    private SpriteRenderer spriteRenderer;
    private Color originalColor = Color.white;
    private Tween colorTween;
    
    // Fever speed variables
    private float originalSpeed;
    private bool hasFeverSpeed = false;
    
    // Fever tag variables
    private string originalTag;
    private bool hasFeverTag = false;

    private Transform waypointParent;

    void OnEnable()
    {
        EnemySpawner.onFever += OnFeverEffects;
        EnemySpawner.onFeverEnd += OnFeverEndEffects;
    }

    void OnDisable()
    {
        EnemySpawner.onFever -= OnFeverEffects;
        EnemySpawner.onFeverEnd -= OnFeverEndEffects;
    }

    void Start()
    {
        // Get SpriteRenderer component and store original color
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        // Store the original tag
        if (string.IsNullOrEmpty(originalTag))
        {
            originalTag = gameObject.tag;
        }
        
        // Always ensure we start clean - stop any existing animations
        StopRGBAnimation();
    }

    public void SetWaypoint(Transform waypoint)
    {
        this.waypoint = waypoint;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
        
        // Store the original speed only if it hasn't been stored yet
        if (originalSpeed == 0f)
        {
            originalSpeed = speed;
        }
    }

    public Transform GetWaypoint()
    {
        return waypoint;
    }

    public void SetWaypointParent(Transform waypointParent)
    {
        this.waypointParent = waypointParent;
        waypoint.parent = waypointParent;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public void OnSpawn()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = true;
        
        if (waypointParent != null)
        {
            waypoint.parent = waypointParent;
        }
        else
        {
            waypoint.parent = null;
        }

        isHooked = false;
        soundPlayed = false;
        isDying = false; // Reset dying state on spawn
        hookedAnimationPlayed = false; // Reset hooked animation state
        facingRight = true; // Reset facing direction on spawn
        hasInitializedFacing = false; // Reset facing initialization
        
        // Reset fever speed state (but don't overwrite originalSpeed)
        hasFeverSpeed = false;
        
        // Reset fever tag state (but don't overwrite originalTag)
        hasFeverTag = false;
        
        // Always stop any existing RGB animation and reset color first
        StopRGBAnimation();
        
        // For pooled objects, we need to check fever status after all components are ready
        StartCoroutine(CheckFeverStatusAfterFrame());
    }
    
    private System.Collections.IEnumerator CheckFeverStatusAfterFrame()
    {
        // Wait two frames to ensure all components are properly initialized
        yield return null;
        yield return null;
        
        // Initialize components if needed
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color;
            }
        }
        
        // Ensure originalSpeed and originalTag are set (fallback safety)
        if (originalSpeed == 0f) originalSpeed = speed;
        if (string.IsNullOrEmpty(originalTag)) originalTag = gameObject.tag;
        
        // Force SpriteAnimator to initialize if it hasn't already
        if (spriteAnimator != null && spriteAnimator.CurrentAnimation == null && currentAnimData != null)
        {
            spriteAnimator.ChangeAnimationObject(currentAnimData);
            spriteAnimator.Play("Idle");
        }
        
        // Apply fever effects or normal state
        if (EnemySpawner.IsFeverActive && feverAnimData != null && spriteAnimator != null)
        {
            yield return new WaitForSeconds(0.1f); // Small delay to ensure animator is ready
            
            ApplyFeverEffects();
        }
        else
        {
            ApplyNormalState();
        }
    }

    private void Awake()
    {
        OnSpawn();
    }

    private void Update()
    {
        Move();
        Hooked();
    }

    private void Move()
    {
        if (isHooked) return;

        Vector3 direction = waypoint.position - transform.position;

        // Flip sprite based on movement direction
        FlipSprite(direction.x);

        transform.position = Vector3.MoveTowards(transform.position, waypoint.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, waypoint.position) < 0.1f)
        {
            waypoint.parent = transform;

            EnemyPool.DestroyEnemy(this);
        }
    }

    private void Hooked()
    {
        if (!isHooked || isDying) return; // Don't play hooked animation if dying

        Vector3 direction = hook.position - transform.position;

        // Flip sprite based on hook direction
        FlipSprite(direction.x);

        transform.position = hook.position;

        // Only play hooked animation once to prevent spam
        if (!hookedAnimationPlayed)
        {
            spriteAnimator.Play("HookHit").SetOnComplete(() =>
            {
                spriteAnimator.Play("Hooked");
            });
            hookedAnimationPlayed = true;
        }

        EnemyHookedSound();
    }

    public void Hook(Transform hook, Vector2 playerPosition)
    {
        this.hook = hook;
        this.playerPosition = playerPosition;
        isHooked = true;
        hookedAnimationPlayed = false; // Reset when newly hooked
    }

    public void EnemyKill()
    {
        waypoint.parent = transform;
        EnemyPool.DestroyEnemy(this);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnPlayerCollide?.Invoke();
            PlayDeathAnimation();

            if (gameObject.tag == "EnemyCursed")
            {
                OnCursedEffect?.Invoke();
                //* cursed explosion sound/effect here
            }
        }

        if (gameObject.tag == "Enemy" && collision.CompareTag("EatArea") || gameObject.tag == "EnemyFever" && collision.CompareTag("EatArea"))
        {
            OnEated?.Invoke();
            PlayDeathAnimation();
            AddScore?.Invoke();
        }

        if (gameObject.tag == "EnemyCursed" && collision.CompareTag("EatArea"))
        {
            OnPlayerCollide?.Invoke();
            OnCursedEffect?.Invoke();
            PlayDeathAnimation();
            //* cursed explosion sound/effect here
        }
    }

    public void PlayDeathAnimation()
    {
        // Set dying state to prevent other animations from interfering
        isDying = true;

        // disable collider
        boxCollider.enabled = false;

        // Play death animation and wait for it to complete before calling EnemyKill
        if (spriteAnimator != null && spriteAnimator.HasAnimation("Dead"))
        {
            EnemyDeadSound();
            spriteAnimator.Play("Dead").SetOnComplete(() =>
            {
                // Death animation completed, now kill the enemy
                EnemyKill();
            });
        }
        else
        {
            EnemyKill();
        }
    }

    #region Sound Methods
    private void EnemyHookedSound()
    {
        // Only play the sound once per enemy
        if (soundPlayed) return;
        soundPlayed = true;

        // play the hook sound once
        AudioManager.Instance.PlaySound(5, 0.5f);
    }

    private void EnemyDeadSound()
    {
        AudioManager.Instance.PlaySound(4);
    }

    private void FlipSprite(float horizontalMovement)
    {
        // Initialize facing direction based on first movement direction
        if (!hasInitializedFacing)
        {
            hasInitializedFacing = true;
            // Set initial facing direction based on movement
            if (horizontalMovement > 0)
            {
                facingRight = true;
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (horizontalMovement < 0)
            {
                facingRight = false;
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            return; // Don't flip on the first frame, just set initial direction
        }
        
        // If moving right and currently facing left, flip to face right
        if (horizontalMovement > 0 && !facingRight)
        {
            facingRight = true;
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        // If moving left and currently facing right, flip to face left
        else if (horizontalMovement < 0 && facingRight)
        {
            facingRight = false;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
    #endregion

    #region Helper Methods
    
    private void ApplyFeverEffects()
    {
        if (spriteAnimator == null || feverAnimData == null) return;
        
        // Change to fever animation
        string currentAnimName = spriteAnimator.CurrentAnimation?.Name ?? "Idle";
        spriteAnimator.ChangeAnimationObject(feverAnimData);
        
        if (spriteAnimator.HasAnimation(currentAnimName))
        {
            spriteAnimator.Play(currentAnimName);
        }
        else
        {
            spriteAnimator.Play("Idle");
        }
        
        // Apply fever effects
        StartRGBAnimation();
        
        if (!hasFeverSpeed)
        {
            speed = originalSpeed * 2f;
            hasFeverSpeed = true;
        }
        
        if (!hasFeverTag)
        {
            gameObject.tag = "EnemyFever";
            hasFeverTag = true;
        }
    }
    
    private void ApplyNormalState()
    {
        if (spriteAnimator == null || currentAnimData == null) return;
        
        // Change to normal animation
        string currentAnimName = spriteAnimator.CurrentAnimation?.Name ?? "Idle";
        spriteAnimator.ChangeAnimationObject(currentAnimData);
        
        if (spriteAnimator.HasAnimation(currentAnimName))
        {
            spriteAnimator.Play(currentAnimName);
        }
        else
        {
            spriteAnimator.Play("Idle");
        }
        
        // Restore normal state
        if (hasFeverSpeed || speed != originalSpeed)
        {
            speed = originalSpeed;
            hasFeverSpeed = false;
        }
        
        if (hasFeverTag || gameObject.tag != originalTag)
        {
            gameObject.tag = originalTag;
            hasFeverTag = false;
        }
    }
    
    #endregion

    #region Effects

    public void OnFeverEffects()
    {
        // Ensure originalSpeed is set (safety check)
        if (originalSpeed == 0f) originalSpeed = speed;
        
        // Apply fever effects immediately if components are ready
        if (spriteAnimator != null && feverAnimData != null)
        {
            ApplyFeverEffects();
        }
        else
        {
            // If components aren't ready, try again with a small delay
            DOVirtual.DelayedCall(0.2f, () =>
            {
                if (spriteAnimator != null && feverAnimData != null)
                {
                    ApplyFeverEffects();
                }
            });
        }
    }

    public void OnFeverEndEffects()
    {
        if (spriteAnimator != null && currentAnimData != null)
        {
            // Revert back to original animation data
            string currentAnimName = spriteAnimator.CurrentAnimation?.Name;
            spriteAnimator.ChangeAnimationObject(currentAnimData);
            
            if (!string.IsNullOrEmpty(currentAnimName) && spriteAnimator.HasAnimation(currentAnimName))
            {
                spriteAnimator.Play(currentAnimName);
            }
            
            // Stop RGB animation and restore original color
            StopRGBAnimation();
            
            // Restore original speed
            if (hasFeverSpeed)
            {
                speed = originalSpeed;
                hasFeverSpeed = false;
            }
            
            // Restore original tag
            if (hasFeverTag)
            {
                gameObject.tag = originalTag;
                hasFeverTag = false;
            }
        }
    }
    
    private void StartRGBAnimation()
    {
        if (spriteRenderer == null) return;
        
        // Kill any existing color tween
        colorTween?.Kill();
        
        // Create soft, pastel RGB colors with reduced intensity
        Color softRed = new Color(1f, 0.7f, 0.7f);        // Soft red
        Color softYellow = new Color(1f, 1f, 0.7f);       // Soft yellow  
        Color softGreen = new Color(0.7f, 1f, 0.7f);      // Soft green
        Color softCyan = new Color(0.7f, 1f, 1f);         // Soft cyan
        Color softBlue = new Color(0.7f, 0.7f, 1f);       // Soft blue
        Color softMagenta = new Color(1f, 0.7f, 1f);      // Soft magenta
        Color softWhite = new Color(0.9f, 0.9f, 0.9f);    // Soft white
        
        // Create a truly seamless soft RGB loop
        colorTween = DOTween.Sequence()
            .Append(spriteRenderer.DOColor(softRed, 0.1f))      // Start → Soft Red
            .Append(spriteRenderer.DOColor(softYellow, 0.1f))   // Red → Soft Yellow
            .Append(spriteRenderer.DOColor(softGreen, 0.1f))    // Yellow → Soft Green
            .Append(spriteRenderer.DOColor(softCyan, 0.1f))     // Green → Soft Cyan
            .Append(spriteRenderer.DOColor(softBlue, 0.1f))     // Cyan → Soft Blue
            .Append(spriteRenderer.DOColor(softMagenta, 0.1f))  // Blue → Soft Magenta
            .Append(spriteRenderer.DOColor(softRed, 0.1f))      // Magenta → Soft Red (complete the circle)
            .Append(spriteRenderer.DOColor(softWhite, 0.1f))    // End with soft white
            .SetLoops(-1, LoopType.Restart)  // Now restart will be smooth
            .SetEase(Ease.InOutSine);
    }
    
    private void StopRGBAnimation()
    {
        if (spriteRenderer == null) return;
        
        // Kill the color tween
        colorTween?.Kill();
        
        // Smoothly return to original color
        spriteRenderer.DOColor(originalColor, 0.3f).SetEase(Ease.InOutQuad);
    }

    #endregion
}