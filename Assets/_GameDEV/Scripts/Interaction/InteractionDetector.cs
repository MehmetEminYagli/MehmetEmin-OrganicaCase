using UnityEngine;
using System;

public class InteractionDetector : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 2f;
    [SerializeField] private float detectionAngle = 90f;
    [SerializeField] private LayerMask interactableLayer;

    public event Action<IInteractable> OnInteractableDetected;
    public event Action OnInteractableLost;

    private IInteractable currentInteractable;

    private void Update()
    {
        DetectInteractables();
    }

    private void DetectInteractables()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, interactableLayer);
        
        IInteractable nearestInteractable = null;
        float nearestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            if (IsInViewAngle(collider.transform.position))
            {
                var interactable = collider.GetComponent<IInteractable>();
                if (interactable != null && interactable.CanInteract)
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestInteractable = interactable;
                        Debug.Log($"Etkileşime girebilecek nesne bulundu: {collider.gameObject.name}");
                    }
                }
            }
        }

        // Handle interactable changes
        if (nearestInteractable != currentInteractable)
        {
            if (currentInteractable != null)
            {
                OnInteractableLost?.Invoke();
            }

            currentInteractable = nearestInteractable;

            if (currentInteractable != null)
            {
                OnInteractableDetected?.Invoke(currentInteractable);
            }
        }
    }

    private bool IsInViewAngle(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToTarget);
        return angle <= detectionAngle * 0.5f;
    }

    public void TryInteract()
    {   
        if (currentInteractable != null)
        {
            if (currentInteractable is InteractableVehicle vehicle)
            {
                Debug.Log("Araç ile etkileşime giriliyor");
                vehicle.SetPlayer(gameObject);
            }
            currentInteractable.Interact();
        }
        else
        {
            Debug.Log("Etkileşime girilecek nesne bulunamadı!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Vector3 rightDir = Quaternion.Euler(0, detectionAngle * 0.5f, 0) * transform.forward;
        Vector3 leftDir = Quaternion.Euler(0, -detectionAngle * 0.5f, 0) * transform.forward;
        
        Gizmos.DrawRay(transform.position, rightDir * detectionRadius);
        Gizmos.DrawRay(transform.position, leftDir * detectionRadius);
    }
} 