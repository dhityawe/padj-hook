using System;
using System.Collections;
using UnityEngine;

public class HookMechanic : MonoBehaviour
{
    private IHookDataProvider hookDataProvider;
    private Rigidbody2D hookRb;
    private Transform hookedEnemy = null;
    private Enemy enemy;
    public static bool isHooking = false;
    public static bool toggleHook = false;
    private bool isOnCooldown = false;

    [SerializeField] private PlayerInput playerInput; // Manually assign in Inspector

    private void Awake()
    {
        hookRb = GetComponent<Rigidbody2D>();
        toggleHook = false;
    }

    public void Initialize(IHookDataProvider provider)
    {
        hookDataProvider = provider;
    }

    public void HookEntity()
    {
        if (isHooking || isOnCooldown || hookDataProvider == null) return;
        
        isHooking = true;
        playerInput.hookSkill.SetActive(false);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector3 playerPos = hookDataProvider.PlayerPosition;  
        float maxRange = hookDataProvider.HookBaseRange;

        float distance = Vector2.Distance(playerPos, mousePos);
        if (distance > maxRange)
        {
            Vector2 direction = (mousePos - playerPos).normalized;
            mousePos = playerPos + (Vector3)direction * maxRange;
        }

        gameObject.SetActive(true);
        hookRb.velocity = (mousePos - transform.position).normalized * hookDataProvider.HookSpeed;

        StartCoroutine(CheckHookDistance(playerPos));
    }

    private IEnumerator CheckHookDistance(Vector3 playerPos)
    {
        while (isHooking)
        {
            float distance = Vector2.Distance(playerPos, transform.position);

            if (distance > hookDataProvider.HookBaseRange)
            {
                ReturnHookSmoothly();
                yield break;
            }
            yield return null;
        }
    }

    private void ReturnHookSmoothly()
    {
        if (hookedEnemy != null)
        {
            StartCoroutine(SnapToEnemy());
        }
        else
        {
            StartCoroutine(SmoothReturn(hookDataProvider.HookPoint.position));
        }
    }

    private IEnumerator SnapToEnemy()
    {
        if (hookedEnemy == null) yield break;

        Vector3 enemyPos = hookedEnemy.position;
        Vector2 direction = (enemyPos - transform.position).normalized;
        hookRb.velocity = direction * hookDataProvider.HookSpeed;

        while (Vector2.Distance(transform.position, enemyPos) > 0.1f)
        {
            yield return null;
        }

        hookRb.velocity = Vector2.zero; // Stop the hook
        hookedEnemy = null;
        StartCoroutine(SmoothReturn(hookDataProvider.HookPoint.position));
    }


    private IEnumerator SmoothReturn(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        float distance = Vector3.Distance(startPos, targetPos);
        float duration = distance / hookDataProvider.HookSpeed;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, time / duration);
            yield return null;
        }

        ResetHook();
    }

    private void ResetHook()
    {
        gameObject.SetActive(false);
        isHooking = false;  

        //** CHANGES MADE HERE !!! - Enemy does not longer die here**//
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (hookedEnemy == null)
            {
                enemy = collision.GetComponent<Enemy>();
                enemy.Hook(this.transform, hookDataProvider.HookPoint.position);
            }

            hookedEnemy = collision.transform;

            ReturnHookSmoothly();
        }
    }
}
