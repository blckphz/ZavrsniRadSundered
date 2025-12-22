using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int attackDamage = 10;

    private Animator animator;
    private PlayerHealth currentTargetHealth;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private bool IsTarget(Collider2D other)
    {
        return other.GetComponent<PlayerHealth>() != null;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsTarget(other))
        {
            currentTargetHealth = other.GetComponent<PlayerHealth>();
            animator.SetBool("IsAttacking", true);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (IsTarget(other))
            animator.SetBool("IsAttacking", true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (IsTarget(other))
        {
            currentTargetHealth = null;
            animator.SetBool("IsAttacking", false);
        }
    }

    public void DealDamage()
    {
        if (currentTargetHealth != null)
            currentTargetHealth.TakeDamage(attackDamage);
    }
}
