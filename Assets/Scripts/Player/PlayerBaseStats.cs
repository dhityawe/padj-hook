using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseStats : MonoBehaviour, IHookDataProvider, IEatDataProvider
{
    [Header("Hook Stats")]
    [SerializeField] private float hookRange = 5f;
    [SerializeField] private float hookSpeed = 10f;
    [SerializeField] private float hookCooldown = 2f; // Cooldown is now set properly
    [SerializeField] private Transform hookPoint;

    [Header("Eat Stats")]
    [SerializeField] private float eatCooldown = 1f;

    // Provide read-only access
    public float HookRange => hookRange;
    public float HookSpeed => hookSpeed;
    public float HookCooldown => hookCooldown; // This was missing before
    public Transform HookPoint => hookPoint;
    public float HookBaseRange => hookRange;
    public Vector3 PlayerPosition => transform.position;
    public float EatCooldown => eatCooldown;
    public int Health = 3;

    [SerializeField] private List<GameObject> hp;

    public static Action OnGameOver;

    void OnEnable()
    {
        Enemy.OnPlayerCollide += DecrementHp;
    }

    void OnDisable()
    {
        Enemy.OnPlayerCollide -= DecrementHp;
    }

    // Draw hook range in editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(HookPoint.transform.position, hookRange);
    }

    public void DecrementHp()
    {
        Health--;
        if (Health >= 0 && Health < hp.Count)
        {
            hp[Health].SetActive(false);
        }
        CheckHp(); // Call CheckHp to trigger game over if health is 0
    }

    public void CheckHp()
    {
        if (Health <= 0)
        {
            OnGameOver?.Invoke();
        }
    }
}
