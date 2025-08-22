using System;
using UnityEngine;
using GabrielBigardi.SpriteAnimator;

public class Enemy : MonoBehaviour
{
    public static Action OnPlayerCollide;
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

        transform.position = Vector3.MoveTowards(transform.position, waypoint.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, waypoint.position) < 0.1f)
        {
            waypoint.parent = transform;

            EnemyPool.DestroyEnemy(this);
        }
    }

    private void StopMove()
    {
        speed = 0;
    }

    private void Hooked()
    {
        if (!isHooked) return;

        transform.position = hook.position;
        // spriteAnimator.Play("Hooked");
        EnemyHookedSound();
    }

    public void Hook(Transform hook, Vector2 playerPosition)
    {
        PlayHookedAnimation();
        this.hook = hook;
        this.playerPosition = playerPosition;
        isHooked = true;
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
            EnemyDeadSound(0.8f);
        }

        if (collision.CompareTag("EatArea"))
        {
            PlayDeathAnimation();
            EnemyDeadSound(0.5f);

            Debug.Log("Enemy got eaten");
        }
    }

    #region Animations

    public void PlayHookedAnimation()
    {
        spriteAnimator?.Play("HookHit")
            .SetOnComplete(() =>
            {
                
                spriteAnimator?.Play("Hooked");
            });
    }

    public void PlayDeathAnimation()
    {
        // disable collider
        boxCollider.enabled = false;

        // Play death animation and wait for it to complete before calling EnemyKill
        if (spriteAnimator != null && spriteAnimator.HasAnimation("Dead"))
        {
            StopMove();
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

    #endregion

    #region Sound Methods
    private void EnemyHookedSound()
    {
        // Only play the sound once per enemy
        if (soundPlayed) return;
        soundPlayed = true;

        // play the hook sound once
        AudioManager.Instance.PlaySound(5, 0.5f);
    }

    private void EnemyDeadSound(float volume)
    {
        AudioManager.Instance.PlaySound(4, volume);
    }
    #endregion
}