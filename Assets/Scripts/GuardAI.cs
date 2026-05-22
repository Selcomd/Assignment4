using UnityEngine;
using UnityEngine.AI;

public class GuardAI : MonoBehaviour
{
    public enum GuardState { Patrol, Investigate, Chase }
    private GuardState currentState = GuardState.Patrol;

    [Header("Patrol Route")]
    public Transform[] patrolPoints;
    public float waypointReachDistance = 1f;
    public float waitTimeAtWaypoint = 1f;

    private int currentPointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    [Header("Vision Cone")]
    public float viewDistance = 10f;
    public float viewAngle = 90f;
    public LayerMask obstacleMask;
    public LayerMask playerMask;

    [Header("Sound Detection")]
    public float hearingRadius = 5f;

    [Header("Chase & Investigate")]
    public float chaseSpeed = 5f;
    public float patrolSpeed = 2f;
    public float investigateSpeed = 3f;
    public float investigateDuration = 5f;

    private Vector3 lastKnownPosition;
    private float investigateTimer = 0f;

    [Header("Catch Settings")]
    public float catchTime = 2f;
    private float detectionTimer = 0f;

    [Header("State Colors")]
    public Color patrolColor = Color.green;
    public Color investigateColor = Color.yellow;
    public Color chaseColor = Color.red;

    private Renderer guardRenderer;

    [Header("Debug")]
    public bool drawPatrolPath = true;
    public bool drawDetectionGizmos = true;

    private NavMeshAgent agent;
    private Transform player;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        guardRenderer = GetComponent<Renderer>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        agent.speed = patrolSpeed;

        if (patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[currentPointIndex].position);

        SetStateColor();
    }

    private void Update()
    {
        bool playerVisible = CanSeePlayer();
        bool playerHeard = CanHearPlayer();

        if (playerVisible)
        {
            detectionTimer += Time.deltaTime;
            if (detectionTimer >= catchTime)
            {
                LossCurtainController lossCurtain = FindFirstObjectByType<LossCurtainController>(FindObjectsInactive.Include);
                if (lossCurtain != null)
                    lossCurtain.StartLossCurtain();
            }
        }
        else
        {
            detectionTimer = 0f;
        }

        switch (currentState)
        {
            case GuardState.Patrol:
                UpdatePatrol();
                if (playerVisible)
                    EnterChase();
                else if (playerHeard)
                    EnterInvestigate(player.position);
                break;

            case GuardState.Investigate:
                UpdateInvestigate();
                if (playerVisible)
                    EnterChase();
                break;

            case GuardState.Chase:
                UpdateChase();
                if (!playerVisible)
                    EnterInvestigate(lastKnownPosition);
                break;
        }
    }

    private void UpdatePatrol()
    {
        if (patrolPoints.Length == 0) return;

        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtWaypoint)
            {
                isWaiting = false;
                waitTimer = 0f;
                MoveToNextPoint();
            }
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= waypointReachDistance)
            isWaiting = true;
    }

    private void MoveToNextPoint()
    {
        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        agent.SetDestination(patrolPoints[currentPointIndex].position);
    }

    private void EnterInvestigate(Vector3 position)
    {
        currentState = GuardState.Investigate;
        lastKnownPosition = position;
        investigateTimer = 0f;
        agent.speed = investigateSpeed;
        agent.SetDestination(lastKnownPosition);
        SetStateColor();
    }

    private void UpdateInvestigate()
    {
        if (!agent.pathPending && agent.remainingDistance <= waypointReachDistance)
        {
            investigateTimer += Time.deltaTime;
            transform.Rotate(0f, 60f * Time.deltaTime, 0f);

            if (investigateTimer >= investigateDuration)
                ReturnToPatrol();
        }
    }

    private void EnterChase()
    {
        currentState = GuardState.Chase;
        agent.speed = chaseSpeed;
        SetStateColor();
    }

    private void UpdateChase()
    {
        if (player == null) return;

        lastKnownPosition = player.position;
        agent.SetDestination(player.position);
    }

    private void ReturnToPatrol()
    {
        currentState = GuardState.Patrol;
        agent.speed = patrolSpeed;

        currentPointIndex = GetNearestPatrolPoint();
        agent.SetDestination(patrolPoints[currentPointIndex].position);
        SetStateColor();
    }

    private int GetNearestPatrolPoint()
    {
        int nearest = 0;
        float minDist = Mathf.Infinity;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, patrolPoints[i].position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = i;
            }
        }

        return nearest;
    }

    private bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > viewDistance) return false;

        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > viewAngle / 2f) return false;

        if (Physics.Raycast(transform.position, directionToPlayer.normalized, distanceToPlayer, obstacleMask))
            return false;

        return true;
    }

    private bool CanHearPlayer()
    {
        if (player == null) return false;

        return Vector3.Distance(transform.position, player.position) <= hearingRadius;
    }

    private void SetStateColor()
    {
        if (guardRenderer == null) return;

        Color stateColor;
        switch (currentState)
        {
            case GuardState.Patrol: stateColor = patrolColor; break;
            case GuardState.Investigate: stateColor = investigateColor; break;
            case GuardState.Chase: stateColor = chaseColor; break;
            default: stateColor = patrolColor; break;
        }

        guardRenderer.material.color = stateColor;

        VisionConeMesh cone = GetComponentInChildren<VisionConeMesh>();
        if (cone != null)
        {
            MeshRenderer coneRenderer = cone.GetComponent<MeshRenderer>();
            if (coneRenderer != null)
            {
                Color coneColor = stateColor;
                coneColor.a = 0.25f;
                coneRenderer.material.color = coneColor;
            }
        }

        SoundRadiusMesh ring = GetComponentInChildren<SoundRadiusMesh>();
        if (ring != null)
        {
            MeshRenderer ringRenderer = ring.GetComponent<MeshRenderer>();
            if (ringRenderer != null)
            {
                Color ringColor = stateColor;
                ringColor.a = 0.7f;
                ringRenderer.material.color = ringColor;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (drawPatrolPath && patrolPoints != null && patrolPoints.Length > 0)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] == null) continue;
                Gizmos.DrawSphere(patrolPoints[i].position, 0.25f);
                Transform next = patrolPoints[(i + 1) % patrolPoints.Length];
                if (next != null) Gizmos.DrawLine(patrolPoints[i].position, next.position);
            }
        }

        if (!drawDetectionGizmos) return;

        Gizmos.color = new Color(0f, 1f, 1f, 0.15f);
        Gizmos.DrawSphere(transform.position, hearingRadius);

        Vector3 forward = transform.forward;

        Vector3 leftEdge = Quaternion.Euler(0, -viewAngle / 2f, 0) * forward * viewDistance;
        Vector3 rightEdge = Quaternion.Euler(0, viewAngle / 2f, 0) * forward * viewDistance;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, leftEdge);
        Gizmos.DrawRay(transform.position, rightEdge);

        int segments = 20;
        float halfAngle = viewAngle / 2f;
        Vector3 prevPoint = transform.position + Quaternion.Euler(0, -halfAngle, 0) * forward * viewDistance;

        Gizmos.color = new Color(1f, 1f, 0f, 0.4f);
        for (int i = 1; i <= segments; i++)
        {
            float t = (float)i / segments;
            float currentAngle = Mathf.Lerp(-halfAngle, halfAngle, t);
            Vector3 nextPoint = transform.position + Quaternion.Euler(0, currentAngle, 0) * forward * viewDistance;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}