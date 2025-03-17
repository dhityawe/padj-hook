using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownManager : MonoBehaviour
{
    private Dictionary<string, float> cooldowns = new Dictionary<string, float>();
    private PlayerBaseStats playerStats;

    public static CooldownManager Instance { get; private set; }

    // Event to notify UI when cooldown updates
    public event Action<string, float> OnCooldownUpdated;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        playerStats = GetComponent<PlayerBaseStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerBaseStats not found on CooldownManager!");
        }
    }

    public bool IsOnCooldown(string action)
    {
        if (cooldowns.TryGetValue(action, out float lastTime))
        {
            return Time.time - lastTime < GetCooldownDuration(action);
        }
        return false;
    }

    public void StartCooldown(string action)
    {
        if (playerStats == null) return; // Prevent errors if playerStats is missing

        float cooldownDuration = GetCooldownDuration(action);
        cooldowns[action] = Time.time;
        Debug.Log($"{action} cooldown started! Ends in {cooldownDuration} seconds.");
        
        // Notify UI immediately
        OnCooldownUpdated?.Invoke(action, cooldownDuration);

        // Start coroutine to track cooldown
        StartCoroutine(UpdateCooldown(action, cooldownDuration));
    }

    private IEnumerator UpdateCooldown(string action, float duration)
    {
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            float remainingTime = duration - (Time.time - startTime);
            OnCooldownUpdated?.Invoke(action, remainingTime); // Notify UI
            yield return null;
        }
        OnCooldownUpdated?.Invoke(action, 0f); // Notify UI cooldown is complete
    }

    private float GetCooldownDuration(string action)
    {
        if (playerStats == null) return 1f;

        return action switch
        {
            "Hook" => playerStats.HookCooldown,
            "Eat" => playerStats.EatCooldown,
            _ => 1f
        };
    }

    public float GetRemainingCooldown(string action)
    {
        if (cooldowns.TryGetValue(action, out float lastTime))
        {
            float duration = GetCooldownDuration(action);
            float elapsedTime = Time.time - lastTime;
            return Mathf.Max(0, duration - elapsedTime);
        }
        return 0;
    }

    public bool IsCooldownReady(string action)
    {
        return GetRemainingCooldown(action) <= 0;
    }
}
