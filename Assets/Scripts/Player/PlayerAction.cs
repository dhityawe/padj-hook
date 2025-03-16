using UnityEngine;
using GabrielBigardi.SpriteAnimator;

public class PlayerActions : MonoBehaviour, IHookDataProvider
{
    [Header("Hook Stats")]
    [SerializeField] private float hookRange = 5f;
    [SerializeField] private float hookSpeed = 10f;
    [SerializeField] private float hookCooldown = 1f;
    [SerializeField] private Transform hookPoint;
    [SerializeField] private GameObject hookObject;

    [Header("Eat Stats")]
    public GameObject eatArea;
    public float eatRange = 5f;
    public float eatCooldown = 1f;
    public SpriteAnimator eatAnimator;

    [Header("Player Ref")]
    public SpriteAnimator playerAnimator;
    private PlayerInput playerInput;
    private HookMechanic hookMechanic;

    private float lastHookTime = -999f;

    // Provide only read-only access to HookMechanic
    public float HookRange => hookRange;
    public float HookSpeed => hookSpeed;
    public float HookCooldown => hookCooldown;
    public Transform HookPoint => hookPoint;
    public float HookBaseRange => hookRange;
    public float LastHookTime => lastHookTime;

    public Vector3 PlayerPosition => transform.position;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        hookMechanic = hookObject.GetComponent<HookMechanic>();

        if (hookMechanic != null)
        {
            hookMechanic.Initialize(this); // Inject dependency
        }

        playerInput.OnHookPressed += HookEntity;
        playerInput.OnEatPressed += EatEntity;
    }

    #region Hook
    void HookEntity()
    {
        if (Time.time - lastHookTime < hookCooldown)
        {
            Debug.Log("Hook on cooldown!");
            return;
        }

        lastHookTime = Time.time; // Set cooldown time
        hookMechanic?.HookEntity();
    }
    #endregion

    #region Eat
    void EatEntity()
    {
        Debug.Log("Eating an entity...");
    }
    #endregion
    
    // Draw gizmos for hook range
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hookRange);
    }
}
