using UnityEngine;
using System;

public class PlayerInput : MonoBehaviour
{
    public event Action OnHookPressed;
    public event Action OnEatPressed;

    public GameObject hookSkill; // Assignable in Inspector
    private PlayerActions playerActions; // Reference to get hook cooldown info

    void Start()
    {
        hookSkill.SetActive(false);
        playerActions = GetComponent<PlayerActions>(); // Get reference to PlayerActions
    }

    void Update()
    {
        // Check hook toggle and cooldown
        if (Input.GetKeyDown(KeyCode.Q) && !HookMechanic.toggleHook && CanUseHook())
        {
            HookMechanic.toggleHook = true;
            hookSkill.SetActive(true);
            Debug.Log("Hook Toggle On!");
        }
        else if (Input.GetKeyDown(KeyCode.Q) && HookMechanic.toggleHook)
        {
            HookMechanic.toggleHook = false;
            hookSkill.SetActive(false);
            Debug.Log("Hook Toggle Off!");
        }

        if (HookMechanic.toggleHook && Input.GetMouseButtonDown(0))
        {
            OnHookPressed?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnEatPressed?.Invoke();
        }
    }

    private bool CanUseHook()
    {
        if (playerActions == null) return false;
        return Time.time - playerActions.LastHookTime >= playerActions.HookCooldown;
    }
}
