using UnityEngine;
using UnityEngine.AI;

public class PatrolGuard : MonoBehaviour
{
    [Header("Patrol Route")]
    public Transform[] patrolPoints;
    public float waypointReachDistance = 1f;
    public float waitTimeAtWaypoint = 1f;

    [Header("Debug")]
    public bool drawPatrolPath = true;

    private NavMeshAgent agent;
    private int currentPointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentPointIndex].position);
        }
    }

    private void Update()
    {
        if (patrolPoints.Length == 0)
        {
            return;
        }

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
        {
            isWaiting = true;
        }
    }

    private void MoveToNextPoint()
    {
        currentPointIndex++;

        if (currentPointIndex >= patrolPoints.Length)
        {
            currentPointIndex = 0;
        }

        agent.SetDestination(patrolPoints[currentPointIndex].position);
    }

    private void OnDrawGizmos()
    {
        if (!drawPatrolPath || patrolPoints == null || patrolPoints.Length == 0)
        {
            return;
        }

        Gizmos.color = Color.green;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i] == null)
            {
                continue;
            }

            Gizmos.DrawSphere(patrolPoints[i].position, 0.25f);

            Transform nextPoint = patrolPoints[(i + 1) % patrolPoints.Length];

            if (nextPoint != null)
            {
                Gizmos.DrawLine(patrolPoints[i].position, nextPoint.position);
            }
        }
    }
}