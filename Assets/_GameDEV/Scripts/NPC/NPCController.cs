using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{
    private NavMeshAgent agent;
    private INPCBehavior currentBehavior;
    private NPCState currentState = NPCState.Idle;
    private VehicleInspectionPoint inspectionPoint;

    [Header("Idle Settings")]
    [SerializeField] private float idleTime = 3f;
    private float idleTimer;

    [Header("Inspection Settings")]
    [SerializeField] private float inspectionDuration = 5f;
    [SerializeField] private float checkNewVehiclesInterval = 3f;
    [SerializeField] private float inspectionDistance = 3f;

    [Header("NavMesh Settings")]
    [SerializeField] private float angularSpeed = 120f;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float moveSpeed = 3.5f;

    private enum NPCState
    {
        Idle,
        Walking,
        Inspecting
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.angularSpeed = angularSpeed;
            agent.acceleration = acceleration;
            agent.speed = moveSpeed;
            agent.stoppingDistance = 0.1f;
            agent.radius = 0.5f;
            agent.height = 2f;
            
            // Y pozisyonu için önemli ayarlar
            agent.baseOffset = 0.6f;  // NPC'nin yerden yüksekliği
            agent.autoTraverseOffMeshLink = false;  // Off-mesh bağlantıları devre dışı bırak
            agent.autoBraking = true;
            
            // NavMesh constraints
            agent.agentTypeID = 0;  // Default agent type
            agent.areaMask = NavMesh.AllAreas;  // Tüm alanlarda hareket edebilir
        }

        // Başlangıç pozisyonunu ayarla
        transform.position = new Vector3(transform.position.x, 0.6f, transform.position.z);
    }

    private void LateUpdate()
    {
        if (agent != null && agent.enabled)
        {
            // NavMeshAgent'in hesapladığı pozisyonu al ama Y'yi koru
            Vector3 newPos = agent.nextPosition;
            newPos.y = 0.6f;
            transform.position = newPos;
        }
    }

    public void Initialize(VehicleInspectionPoint inspectionPoint)
    {
        this.inspectionPoint = inspectionPoint;
        SetNewBehavior(CreateNewInspectorBehavior());
    }

    private NPCVehicleInspector CreateNewInspectorBehavior()
    {
        var inspector = new NPCVehicleInspector();
        inspector.Initialize(
            inspectionDuration,
            checkNewVehiclesInterval,
            inspectionDistance,
            inspectionPoint
        );
        return inspector;
    }

    private void Update()
    {
        if (inspectionPoint == null) return;

        switch (currentState)
        {
            case NPCState.Idle:
                HandleIdleState();
                break;
            case NPCState.Walking:
            case NPCState.Inspecting:
                HandleActiveBehavior();
                break;
        }
    }

    private void HandleIdleState()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleTime)
        {
            idleTimer = 0f;
            SetNewBehavior(CreateNewInspectorBehavior());
        }
    }

    private void HandleActiveBehavior()
    {
        if (currentBehavior != null)
        {
            currentBehavior.ExecuteBehavior(transform);
            
            if (currentBehavior.IsBehaviorComplete)
            {
                currentBehavior.OnBehaviorComplete();
                currentState = NPCState.Idle;
                currentBehavior = null;
            }
        }
    }

    private void SetNewBehavior(INPCBehavior behavior)
    {
        currentBehavior = behavior;
        currentBehavior.OnBehaviorStart();
        currentState = NPCState.Walking;
    }

    public void StopCurrentBehavior()
    {
        if (currentBehavior != null)
        {
            currentBehavior.OnBehaviorComplete();
            currentBehavior = null;
        }
        currentState = NPCState.Idle;
        agent.ResetPath();
    }

    public NavMeshAgent GetAgent()
    {
        return agent;
    }
} 