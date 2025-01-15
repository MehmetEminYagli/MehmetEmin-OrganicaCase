using UnityEngine;

public abstract class InteractableObject : MonoBehaviour, IInteractable
{
    [SerializeField] protected bool isInteractable = true;

    public virtual bool CanInteract => isInteractable;

    public abstract void Interact();

    protected virtual void OnInteractionComplete()
    {
        // Base implementation can be overridden by derived classes
    }
} 