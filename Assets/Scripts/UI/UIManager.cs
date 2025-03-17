using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider hookCooldownSlider;
    [SerializeField] private Slider eatCooldownSlider;
    Coroutine hookCooldownCoroutine;
    Coroutine eatCooldownCoroutine;

    private void OnEnable()
    {
        // Ensure CooldownManager is initialized before subscribing
        if (CooldownManager.Instance != null)
        {
            CooldownManager.Instance.OnCooldownStart += HandleCooldownStart;
        }
        else
        {
            Debug.LogError("CooldownManager is not initialized yet!");
        }
    }


    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        CooldownManager.Instance.OnCooldownStart -= HandleCooldownStart;
    }

    private void Start()
    {
        // Initialize cooldown sliders to 0 (ready state)
        hookCooldownSlider.value = 0;
        eatCooldownSlider.value = 0;
    }

    private void HandleCooldownStart(string skillName, float duration)
    {
        if (skillName == "Hook")
        {        
            if (hookCooldownCoroutine != null)
                {
                    StopCoroutine(hookCooldownCoroutine);
                }
            hookCooldownCoroutine = StartCoroutine(UpdateCooldown(hookCooldownSlider, duration));
        }
        else if (skillName == "Eat")
        {
            if (eatCooldownCoroutine != null)
            {
                StopCoroutine(eatCooldownCoroutine);
            }
            eatCooldownCoroutine = StartCoroutine(UpdateCooldown(eatCooldownSlider, duration));
        }
    }

    private IEnumerator UpdateCooldown(Slider cooldownSlider, float cooldownTime)
    {
        cooldownSlider.value = 1f; // Ensure it starts at full value
        yield return null; // Allow UI to update

        int steps = 10; // Always exactly 10 steps
        float decrementAmount = 1f / steps; // Fixed decrease per step (0.1)
        float decrementInterval = cooldownTime / steps; // Time between each decrement

        for (int i = 1; i <= steps; i++) // Ensure exactly 10 updates
        {
            yield return new WaitForSeconds(decrementInterval); // Wait for the interval
            cooldownSlider.value = 1f - (decrementAmount * i); // Decrease in fixed steps
        }

        cooldownSlider.value = 0f; // Ensure it reaches exactly 0
    }
}
