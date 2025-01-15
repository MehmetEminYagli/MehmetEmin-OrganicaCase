using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float movementSmoothness = 0.1f;
    [SerializeField] private InteractionDetector interactionDetector;

    private Vector3 moveDirection;
    private Vector3 currentVelocity;
    private Vector3 smoothVelocity;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        if (Input.GetKeyDown(KeyCode.E) && interactionDetector != null)
        {
            Debug.Log("E tuşuna basıldı");
            interactionDetector.TryInteract();
        }
    }

    private void FixedUpdate()
    {
        if (moveDirection != Vector3.zero)
        {
            // Smooth movement using Vector3.SmoothDamp
            Vector3 targetVelocity = moveDirection * moveSpeed;
            currentVelocity = Vector3.SmoothDamp(currentVelocity, targetVelocity, ref smoothVelocity, movementSmoothness);
            rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);

            // Smooth rotation
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            rb.MoveRotation(Quaternion.Lerp(rb.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime));
        }
        else
        {
            // Gradually slow down when no input
            currentVelocity = Vector3.SmoothDamp(currentVelocity, Vector3.zero, ref smoothVelocity, movementSmoothness);
            rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
        }
    }
} 