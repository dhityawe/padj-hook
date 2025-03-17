using UnityEngine;

public interface IHookDataProvider
{
    float HookBaseRange { get; }  // Base range, but distance is recalculated dynamically
    float HookSpeed { get; }
    float HookCooldown { get; }
    Vector3 PlayerPosition { get; }  // Add player position reference
    Transform HookPoint { get; }

}


