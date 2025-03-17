using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

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

    private void HandleCooldownStart(string skillName, float currentTime, float duration)
    {
        if (skillName == "Hook")
        {        
            hookCooldownSlider.value = Mathf.Lerp(0, 1, currentTime / duration);
        }

        else if (skillName == "Eat")
        {
            eatCooldownSlider.value = Mathf.Lerp(0, 1, currentTime / duration);
        }
    }

    private IEnumerator UpdateCooldown(Slider cooldownSlider, float cooldownTime)
    {
        cooldownSlider.value = 1;
        float currentTime = cooldownTime;

        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            cooldownSlider.value = Mathf.Lerp(0, 1, currentTime / cooldownTime);
            //Debug.Log(Mathf.Lerp(0, 1, currentTime / cooldownTime));
            yield return null;
        }

        cooldownSlider.value = 0;
    }
}
