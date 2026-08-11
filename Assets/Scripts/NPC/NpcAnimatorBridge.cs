using UnityEngine;

public class NpcAnimatorBridge : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NpcNavAgent navAgent;
    [SerializeField] private string speedParameter = "Speed";
    [SerializeField] private string sitTrigger = "Sit";
    private void Update() { if (animator != null && navAgent != null) animator.SetFloat(speedParameter, navAgent.CurrentSpeed); }
    public void PlaySit() { if (animator != null) animator.SetTrigger(sitTrigger); }
}
