using System;
using UnityEngine;
using GabrielBigardi.SpriteAnimator;

public class Enemy : MonoBehaviour
{
    public static Action OnPlayerCollide;
    public static Action OnEated;
    public static Action OnCursedEffect;
    public SpriteAnimator spriteAnimator;
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

    private Transform waypointParent;

    public void SetWaypoint(Transform waypoint)
    {
        this.waypoint = waypoint;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
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
            spriteAnimator.Play("HookHit").SetOnComplete(() => {
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

            PlayerBaseStats playerStats = collision.GetComponent<PlayerBaseStats>();
            if (playerStats != null)
            {
                playerStats.Health -= 1;
            }
            else
            {
                Debug.LogWarning("Player collider doesn't have PlayerBaseStats component!");
            }
        }

        if (gameObject.tag == "Enemy" && collision.CompareTag("EatArea"))
        {
            OnEated?.Invoke();
            PlayDeathAnimation();
        }

        if (gameObject.tag == "EnemyCursed" && collision.CompareTag("EatArea"))
        {
            OnPlayerCollide?.Invoke();
            OnCursedEffect?.Invoke();
            PlayDeathAnimation();
            //* cursed explosion sound/effect here
            
            PlayerBaseStats playerStats = collision.GetComponent<PlayerBaseStats>();
            if (playerStats != null)
            {
                playerStats.Health -= 1;
            }
            else
            {
                Debug.LogWarning("Player collider doesn't have PlayerBaseStats component!");
            }
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
            // If no death animation exists, just kill immediately
            Debug.LogWarning("No 'Dead' animation found, killing enemy immediately");
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
}