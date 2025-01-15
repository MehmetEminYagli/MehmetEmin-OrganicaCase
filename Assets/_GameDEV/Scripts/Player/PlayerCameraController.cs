using UnityEngine;
using Cinemachine;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Cinemachine Camera References")]
    [SerializeField] private CinemachineVirtualCamera firstPersonVCam;
    [SerializeField] private CinemachineVirtualCamera thirdPersonVCam;

    private bool isFirstPerson = true;

    private void Start()
    {
        // Ensure virtual cameras are set up correctly at start
        if (firstPersonVCam != null && thirdPersonVCam != null)
        {
            SetCameraPriorities(isFirstPerson);
        }
        else
        {
            Debug.LogError("Virtual Cameras are not assigned in PlayerCameraController!");
        }
    }

    private void Update()
    {
        // Handle camera switching
        if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchCamera();
        }
    }

    private void SwitchCamera()
    {
        isFirstPerson = !isFirstPerson;
        SetCameraPriorities(isFirstPerson);
    }

    private void SetCameraPriorities(bool isFirstPersonActive)
    {
        // Cinemachine uses priorities to determine which virtual camera is active
        // Higher priority (11) becomes the active camera
        if (firstPersonVCam != null)
            firstPersonVCam.Priority = isFirstPersonActive ? 11 : 10;
        
        if (thirdPersonVCam != null)
            thirdPersonVCam.Priority = isFirstPersonActive ? 10 : 11;
    }
} 