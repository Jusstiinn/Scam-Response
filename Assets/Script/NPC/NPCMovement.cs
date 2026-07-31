using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Action arrivalCallback;

    public bool HasArrived =>
        agent != null &&
        !agent.pathPending &&
        agent.remainingDistance <= agent.stoppingDistance &&
        (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f);

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (arrivalCallback != null && HasArrived)
        {
            Action callback = arrivalCallback;
            arrivalCallback = null;
            callback.Invoke();
        }
    }

    public void MoveTo(Transform destination, Action onArrived = null)
    {
        if (destination == null || agent == null || !agent.isOnNavMesh)
            return;

        arrivalCallback = onArrived;
        agent.SetDestination(destination.position);
    }

    public void Stop()
    {
        arrivalCallback = null;

        if (agent != null && agent.isOnNavMesh)
            agent.ResetPath();
    }

    public void Warp(Vector3 position)
    {
        arrivalCallback = null;

        if (agent != null && agent.isOnNavMesh)
            agent.Warp(position);
        else
            transform.position = position;
    }
}
