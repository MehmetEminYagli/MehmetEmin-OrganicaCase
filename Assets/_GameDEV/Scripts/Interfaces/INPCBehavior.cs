using UnityEngine;

public interface INPCBehavior
{
    void ExecuteBehavior(Transform npcTransform);
    bool IsBehaviorComplete { get; }
    void OnBehaviorStart();
    void OnBehaviorComplete();
} 