using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NpcNavAgent : MonoBehaviour
{
    private NavMeshAgent agent;
    private Action arrival;

    private bool waitingForArrival;
    private Vector3 requestedDestination;

    public float CurrentSpeed =>
        agent != null ? agent.velocity.magnitude : 0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (!waitingForArrival ||
            arrival == null ||
            !agent.isOnNavMesh)
        {
            return;
        }

        // Unity is still calculating the route.
        if (agent.pathPending)
            return;

        // We need an actual path unless we are already at the target.
        if (!agent.hasPath)
        {
            float directDistance =
                Vector3.Distance(
                    transform.position,
                    requestedDestination
                );

            if (directDistance >
                agent.stoppingDistance + 0.1f)
            {
                return;
            }
        }

        if (agent.remainingDistance >
            agent.stoppingDistance + 0.05f)
        {
            return;
        }

        if (agent.velocity.sqrMagnitude > 0.01f)
            return;

        waitingForArrival = false;

        Action callback = arrival;
        arrival = null;

        callback?.Invoke();
    }

    public void MoveTo(
        Transform destination,
        Action onArrived = null)
    {
        if (destination == null)
        {
            Debug.LogWarning(
                "NpcNavAgent: Destination is null.",
                this
            );
            return;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning(
                "NpcNavAgent: Agent is not on NavMesh.",
                this
            );
            return;
        }

        requestedDestination = destination.position;
        arrival = onArrived;

        float distance =
            Vector3.Distance(
                transform.position,
                requestedDestination
            );

        // Truly already at the destination.
        if (distance <=
            agent.stoppingDistance + 0.05f)
        {
            waitingForArrival = false;

            Action callback = arrival;
            arrival = null;

            callback?.Invoke();
            return;
        }

        waitingForArrival = true;

        agent.isStopped = false;
        agent.ResetPath();

        bool success =
            agent.SetDestination(
                requestedDestination
            );

        if (!success)
        {
            waitingForArrival = false;
            arrival = null;

            Debug.LogError(
                "NpcNavAgent could not set destination.",
                this
            );
        }
    }

    public void Follow(Transform target)
    {
        if (target == null ||
            !agent.isOnNavMesh)
        {
            return;
        }

        waitingForArrival = false;
        arrival = null;

        agent.isStopped = false;
        agent.SetDestination(target.position);
    }

    public void Warp(Vector3 position)
    {
        waitingForArrival = false;
        arrival = null;

        if (agent.isOnNavMesh)
            agent.Warp(position);
        else
            transform.position = position;
    }

    public void SetSpeed(float speed)
    {
        if (agent != null)
            agent.speed = speed;
    }
}