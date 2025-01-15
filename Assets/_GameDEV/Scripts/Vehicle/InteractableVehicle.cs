using UnityEngine;
using System;

public class InteractableVehicle : InteractableObject
{
    [Header("Vehicle Settings")]
    [SerializeField] private Transform playerSeatPosition;
    [SerializeField] private Transform exitPosition;
    [SerializeField] private GameObject vehicleCamera;
    [SerializeField] private MonoBehaviour carController;
    
    [Header("State")]
    [SerializeField] private bool isOccupied = false;

    private Transform playerTransform;
    private GameObject playerObject;
    private CharacterController playerController;
    private PlayerMovement playerMovement;
    private Collider playerCollider;
    private Renderer[] playerRenderers;


    public event Action<bool> OnVehicleStateChanged;

    private void Start()
    {
        if (vehicleCamera != null)
        {
            vehicleCamera.SetActive(false);
        }

        if (carController != null)
        {
            carController.enabled = false;
        }
    }

    public void SetPlayer(GameObject player)
    {
        playerObject = player;
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            playerObject.TryGetComponent(out playerController);
            playerObject.TryGetComponent(out playerMovement);
            playerObject.TryGetComponent(out playerCollider);
            playerRenderers = playerObject.GetComponentsInChildren<Renderer>();
            
        }
    }

    public override void Interact()
    {
        if (!isOccupied)
        {
            EnterVehicle();
        }
        else
        {
            ExitVehicle();
        }
    }

    private void EnterVehicle()
    {
        if (playerObject == null) return;

        // Disable player components
        if (playerController != null) playerController.enabled = false;
        if (playerMovement != null) playerMovement.enabled = false;
        if (playerCollider != null) playerCollider.enabled = false;
        foreach (var renderer in playerRenderers)
        {
            renderer.enabled = false;
        }
       

        // Position player at seat
        playerTransform.position = playerSeatPosition.position;
        playerTransform.rotation = playerSeatPosition.rotation;
        playerTransform.parent = transform;

        // Enable vehicle camera, disable player camera
        if (vehicleCamera != null)
        {
            Camera playerCamera = playerObject.GetComponentInChildren<Camera>();
            if (playerCamera != null) playerCamera.gameObject.SetActive(false);
            vehicleCamera.SetActive(true);
        }

        // Araç kontrolcüsünü etkinleştir
        if (carController != null)
        {
            carController.enabled = true;
        }

        isOccupied = true;
        OnVehicleStateChanged?.Invoke(true);
    }

    private void ExitVehicle()
    {
        if (playerTransform == null) return;

        // Araç kontrolcüsünü devre dışı bırak
        if (carController != null)
        {
            carController.enabled = false;
        }

        // Reset player parent
        playerTransform.parent = null;

        // Position player at exit
        playerTransform.position = exitPosition.position;
        playerTransform.rotation = exitPosition.rotation;

        // Re-enable player components
        if (playerController != null) playerController.enabled = true;
        if (playerMovement != null) playerMovement.enabled = true;
        if (playerCollider != null) playerCollider.enabled = true;
        foreach (var renderer in playerRenderers)
        {
            renderer.enabled = true;
        }
       

        // Switch cameras back
        if (vehicleCamera != null)
        {
            vehicleCamera.SetActive(false);
            Camera playerCamera = playerObject.GetComponentInChildren<Camera>();
            if (playerCamera != null) playerCamera.gameObject.SetActive(true);
        }

        isOccupied = false;
        OnVehicleStateChanged?.Invoke(false);
    }

    public void Update()
    {
        if (isOccupied && Input.GetKeyDown(KeyCode.E))
        {
            ExitVehicle();
        }
    }

    public override bool CanInteract => !isOccupied;
} 