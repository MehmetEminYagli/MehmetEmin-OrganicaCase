using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class NPCVehicleInspector : INPCBehavior
{
    private enum InspectionState
    {
        FindingInspectionPoint,
        MovingToVehicle,
        Inspecting,
        Completed
    }

    private InspectionState currentState;
    private float inspectionTimer;
    private float checkNewVehiclesTimer;
    private Vector3 lastValidPosition;
    private Transform lastInspectedVehicle;
    private Vector3 currentInspectionPoint;
    private int consecutiveInspections = 0;
    private const int MAX_CONSECUTIVE_INSPECTIONS = 2;
    private const int ANGLE_SEGMENTS = 8; // Aracın etrafındaki bölüm sayısı
    
    private float inspectionDuration;
    private float checkNewVehiclesInterval;
    private float inspectionDistance;
    private Transform targetVehicle;
    private VehicleInspectionPoint inspectionPoint;

    // Yapay zeka için ilgi faktörleri
    private float interestInCurrentVehicle = 1f;
    private const float INTEREST_DECAY_RATE = 0.2f;
    private const float MIN_INTEREST = 0.3f;
    private const float NEW_VEHICLE_INTEREST_BONUS = 0.5f;

    // Tüm NPC'lerin kullandığı noktaları takip etmek için statik liste
    private static Dictionary<Vector3, float> occupiedPoints = new Dictionary<Vector3, float>();
    private const float POINT_OCCUPATION_RADIUS = 2.5f;
    private const float POINT_CLEAR_TIME = 0.5f;
    private const float MIN_DISTANCE_BETWEEN_NPCS = 2f;

    private const float FIXED_Y_POSITION = 0.6f; // Y pozisyonunu 0.6f olarak değiştirdik

    public bool IsBehaviorComplete => currentState == InspectionState.Completed;

    public void Initialize(
        float inspectionDuration,
        float checkNewVehiclesInterval,
        float inspectionDistance,
        VehicleInspectionPoint inspectionPoint)
    {
        this.inspectionDuration = inspectionDuration;
        this.checkNewVehiclesInterval = checkNewVehiclesInterval;
        this.inspectionDistance = inspectionDistance;
        this.inspectionPoint = inspectionPoint;
        ResetInterest();
    }

    private void ResetInterest()
    {
        interestInCurrentVehicle = 1f;
        consecutiveInspections = 0;
    }

    public void OnBehaviorStart()
    {
        currentState = InspectionState.FindingInspectionPoint;
        SelectRandomVehicle();
        ResetTimers();
    }

    private void ResetTimers()
    {
        inspectionTimer = 0f;
        checkNewVehiclesTimer = 0f;
    }

    public void OnBehaviorComplete()
    {
        if (currentInspectionPoint != Vector3.zero)
        {
            RemoveOccupiedPoint(currentInspectionPoint);
        }
        currentState = InspectionState.Completed;
        ResetTimers();
        lastInspectedVehicle = targetVehicle;
    }

    private static void AddOccupiedPoint(Vector3 point)
    {
        // Yakındaki eski noktaları temizle
        var pointsToRemove = new List<Vector3>();
        foreach (var kvp in occupiedPoints)
        {
            if (Vector3.Distance(point, kvp.Key) < MIN_DISTANCE_BETWEEN_NPCS)
            {
                pointsToRemove.Add(kvp.Key);
            }
        }

        foreach (var oldPoint in pointsToRemove)
        {
            occupiedPoints.Remove(oldPoint);
        }

        // Yeni noktayı ekle
        occupiedPoints[point] = Time.time;
    }

    private static void RemoveOccupiedPoint(Vector3 point)
    {
        if (occupiedPoints.ContainsKey(point))
        {
            occupiedPoints.Remove(point);
        }
    }

    private static bool IsPointOccupied(Vector3 point)
    {
        foreach (var occupiedPoint in occupiedPoints.Keys)
        {
            if (Vector3.Distance(point, occupiedPoint) < POINT_OCCUPATION_RADIUS)
            {
                return true;
            }
        }
        return false;
    }

    private static void CleanupOldPoints()
    {
        var currentTime = Time.time;
        var pointsToRemove = new List<Vector3>();

        foreach (var kvp in occupiedPoints)
        {
            if (currentTime - kvp.Value > POINT_CLEAR_TIME)
            {
                pointsToRemove.Add(kvp.Key);
            }
        }

        foreach (var point in pointsToRemove)
        {
            occupiedPoints.Remove(point);
        }
    }

    public void ExecuteBehavior(Transform npcTransform)
    {
        if (targetVehicle == null)
        {
            OnBehaviorComplete();
            return;
        }

        CleanupOldPoints();

        if (currentState == InspectionState.Inspecting)
        {
            interestInCurrentVehicle = Mathf.Max(MIN_INTEREST, 
                interestInCurrentVehicle - INTEREST_DECAY_RATE * Time.deltaTime);
        }

        checkNewVehiclesTimer += Time.deltaTime;
        if (checkNewVehiclesTimer >= checkNewVehiclesInterval && currentState == InspectionState.Inspecting)
        {
            checkNewVehiclesTimer = 0f;
            ConsiderNewVehicle();
        }

        switch (currentState)
        {
            case InspectionState.FindingInspectionPoint:
                FindInspectionPoint(npcTransform);
                break;
            case InspectionState.MovingToVehicle:
                HandleMovement(npcTransform);
                break;
            case InspectionState.Inspecting:
                HandleInspection(npcTransform);
                break;
        }
    }

    private Vector3 AdjustYPosition(Vector3 position)
    {
        return new Vector3(position.x, FIXED_Y_POSITION, position.z);
    }

    private void FindInspectionPoint(Transform npcTransform)
    {
        List<Vector3> availablePoints = new List<Vector3>();
        float[] radiusOptions = { inspectionDistance, inspectionDistance * 1.25f, inspectionDistance * 1.5f };

        foreach (float radius in radiusOptions)
        {
            // Her yarıçap için rastgele bir başlangıç açısı seç
            float randomStartAngle = Random.Range(0f, 360f);
            
            for (int i = 0; i < ANGLE_SEGMENTS; i++)
            {
                float angle = randomStartAngle + i * (360f / ANGLE_SEGMENTS);
                float radian = angle * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian)) * radius;
                Vector3 potentialPoint = new Vector3(
                    targetVehicle.position.x + offset.x,
                    FIXED_Y_POSITION,
                    targetVehicle.position.z + offset.z
                );

                NavMeshHit hit;
                if (NavMesh.SamplePosition(potentialPoint, out hit, 2f, NavMesh.AllAreas))
                {
                    Vector3 adjustedPoint = new Vector3(hit.position.x, FIXED_Y_POSITION, hit.position.z);

                    // Diğer NPC'lerle çakışma kontrolü
                    bool isPointValid = true;
                    foreach (var kvp in occupiedPoints)
                    {
                        if (Vector3.Distance(adjustedPoint, kvp.Key) < MIN_DISTANCE_BETWEEN_NPCS)
                        {
                            isPointValid = false;
                            break;
                        }
                    }

                    // Platform üzerinde olup olmadığını kontrol et
                    if (isPointValid)
                    {
                        RaycastHit platformHit;
                        if (!Physics.Raycast(adjustedPoint + Vector3.up * 2f, Vector3.down, out platformHit, 5f, LayerMask.GetMask("Platform")))
                        {
                            availablePoints.Add(adjustedPoint);
                        }
                    }
                }
            }

            // Eğer bu yarıçapta yeterli nokta bulunduysa döngüyü sonlandır
            if (availablePoints.Count >= 3)
            {
                break;
            }
        }

        if (availablePoints.Count > 0)
        {
            // Rastgele bir nokta seç (en yakın nokta yerine)
            int randomIndex = Random.Range(0, availablePoints.Count);
            Vector3 selectedPoint = availablePoints[randomIndex];

            // Önceki noktayı temizle ve yeni noktayı işgal et
            if (currentInspectionPoint != Vector3.zero)
            {
                RemoveOccupiedPoint(currentInspectionPoint);
            }

            currentInspectionPoint = selectedPoint;
            AddOccupiedPoint(currentInspectionPoint);
            currentState = InspectionState.MovingToVehicle;
        }
        else
        {
            // Uygun nokta bulunamadıysa biraz bekle ve tekrar dene
            StartNewInspection();
        }
    }

    private void HandleMovement(Transform npcTransform)
    {
        NavMeshAgent agent = npcTransform.GetComponent<NavMeshAgent>();
        if (agent == null) return;

        // NPC'nin y pozisyonunu zorla
        Vector3 currentPos = npcTransform.position;
        npcTransform.position = AdjustYPosition(currentPos);

        float distanceToTarget = Vector3.Distance(npcTransform.position, currentInspectionPoint);
        if (distanceToTarget <= agent.stoppingDistance + 0.1f)
        {
            currentState = InspectionState.Inspecting;
            lastValidPosition = AdjustYPosition(npcTransform.position);
            consecutiveInspections++;
            
            Vector3 directionToVehicle = (targetVehicle.position - npcTransform.position).normalized;
            npcTransform.forward = new Vector3(directionToVehicle.x, 0, directionToVehicle.z).normalized;
            
            agent.ResetPath();
        }
        else
        {
            Vector3 targetPosition = AdjustYPosition(currentInspectionPoint);
            agent.SetDestination(targetPosition);
        }
    }

    private void StartNewInspection()
    {
        if (currentInspectionPoint != Vector3.zero)
        {
            RemoveOccupiedPoint(currentInspectionPoint);
        }
        currentState = InspectionState.FindingInspectionPoint;
        ResetTimers();
    }

    private void ConsiderNewVehicle()
    {
        Transform newTarget = inspectionPoint.GetRandomSalePoint();
        if (newTarget != null && newTarget != targetVehicle && newTarget != lastInspectedVehicle)
        {
            float switchProbability = CalculateSwitchProbability(newTarget);
            if (Random.value < switchProbability)
            {
                if (currentInspectionPoint != Vector3.zero)
                {
                    RemoveOccupiedPoint(currentInspectionPoint);
                }
                targetVehicle = newTarget;
                currentState = InspectionState.FindingInspectionPoint;
                ResetTimers();
                interestInCurrentVehicle = 1f + NEW_VEHICLE_INTEREST_BONUS;
                consecutiveInspections = 0;
            }
        }
    }

    private float CalculateSwitchProbability(Transform newTarget)
    {
        float baseProbability = 0.3f;
        baseProbability += (1f - interestInCurrentVehicle);

        if (consecutiveInspections >= MAX_CONSECUTIVE_INSPECTIONS)
        {
            baseProbability += 0.3f;
        }

        float distanceToNew = Vector3.Distance(targetVehicle.position, newTarget.position);
        if (distanceToNew < 5f)
        {
            baseProbability += 0.2f;
        }

        return Mathf.Clamp01(baseProbability);
    }

    private void HandleInspection(Transform npcTransform)
    {
        // NPC'nin y pozisyonunu zorla
        Vector3 currentPos = npcTransform.position;
        npcTransform.position = AdjustYPosition(currentPos);

        if (targetVehicle != null)
        {
            Vector3 directionToVehicle = (targetVehicle.position - npcTransform.position).normalized;
            npcTransform.forward = new Vector3(directionToVehicle.x, 0, directionToVehicle.z).normalized;
        }

        inspectionTimer += Time.deltaTime;
        float adjustedInspectionDuration = inspectionDuration * interestInCurrentVehicle;
        
        if (inspectionTimer >= adjustedInspectionDuration)
        {
            OnBehaviorComplete();
        }
    }

    private void SelectRandomVehicle()
    {
        if (inspectionPoint != null)
        {
            Transform newTarget = inspectionPoint.GetRandomSalePoint();
            
            if (newTarget != null && newTarget != lastInspectedVehicle)
            {
                targetVehicle = newTarget;
                ResetInterest();
            }
        }
        else
        {
            Debug.LogError("VehicleInspectionPoint referansı eksik!");
        }
    }
} 