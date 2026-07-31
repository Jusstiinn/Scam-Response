using UnityEngine;

public class NPCAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string speedParameter = "Speed";

    private NPCMovement movement;

    private void Awake()
    {
        movement = GetComponent<NPCMovement>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (animator == null)
            return;

        float speed = 0f;
        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (agent != null)
            speed = agent.velocity.magnitude;

        animator.SetFloat(speedParameter, speed);
    }
}
