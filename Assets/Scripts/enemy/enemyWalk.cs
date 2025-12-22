using UnityEngine;
using Pathfinding;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(enemyFindTarget))]
public class enemyWalk : MonoBehaviour
{
    public float speed = 2f;
    public Animator animator;
    public float pathUpdateInterval = 0.2f;
    public float aggroRange = 5f;

    public bool isAggressive = true;

    private Seeker seeker;
    private Rigidbody2D rb;
    private Path path;
    private int currentWaypoint = 0;
    private float nextWaypointDistance = 0.1f;

    public float attackRange = 1f;
    public float attackRate = 2f;
    public int attackDamage = 10;
    private float nextAttackTime = 0f;

    private enemyFindTarget targetFinder;
    private bool isAggroed_internal = false;

    public bool isRangedEnemy = false;
    public float fleeDistance = 2.5f;
    public float safeDistance = 4f;

    private GameObject fleeGoal;

    void Awake()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        targetFinder = GetComponent<enemyFindTarget>();

        fleeGoal = new GameObject(gameObject.name + "_FleeGoal");
        fleeGoal.SetActive(false);

    }

    void Update()
    {
        if (targetFinder == null) return;

        Transform target = targetFinder.currentTarget;

        if (target == null)
        {
            StopChasing();
            return;
        }

        float distance = Vector2.Distance(rb.position, target.position);

        if (!isAggressive && !isAggroed_internal)
            return;

        if (isAggressive && !isAggroed_internal && distance <= aggroRange)
            StartChasing();

        if (!isAggroed_internal)
            return;

        if (isRangedEnemy)
            RangedLogic(target, distance);
        else
            MeleeLogic(target, distance);
    }

    private void MeleeLogic(Transform target, float distance)
    {
        if (distance <= attackRange)
        {
            StopMovement();
            Vector2 dir = (target.position - transform.position).normalized;
            SetAnimatorDirection(dir);

            if (Time.time >= nextAttackTime)
            {
                PerformAttack(target);
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
        else
        {
            MoveAlongPath();
        }
    }

    private void RangedLogic(Transform target, float distance)
    {
        if (distance < fleeDistance)
        {
            if (!fleeGoal.activeSelf || Vector2.Distance(fleeGoal.transform.position, target.position) < safeDistance)
            {
                ResetFleeGoal();
                CreateFleeGoal(target);
            }

            seeker.StartPath(rb.position, fleeGoal.transform.position, OnPathComplete);
            MoveAlongPath();
            return;
        }

        if (distance <= safeDistance)
        {
            ResetFleeGoal();
            StopMovement();
            LookAtTarget(target);

            if (Time.time >= nextAttackTime)
            {
                PerformAttack(target);
                nextAttackTime = Time.time + 1f / attackRate;
            }
            return;
        }

        ResetFleeGoal();
        MoveAlongPath();
    }

    private void StartChasing()
    {
        if (isAggroed_internal)
            return;

        isAggroed_internal = true;
        PlayerCombat.RegisterAggro(this.transform);

        StartCoroutine(UpdatePathRoutine());
    }

    private void StopChasing()
    {
        if (!isAggroed_internal)
            return;

        isAggroed_internal = false;
        PlayerCombat.DeregisterAggro(this.transform);

        StopMovement();
        StopAllCoroutines();

        if (seeker != null)
            seeker.CancelCurrentPathRequest();

        path = null;
        currentWaypoint = 0;
    }

    private void OnDestroy()
    {
        PlayerCombat.DeregisterAggro(this.transform);

        if (fleeGoal != null)
            Destroy(fleeGoal);
    }

    private IEnumerator UpdatePathRoutine()
    {
        while (isAggroed_internal && targetFinder.currentTarget != null)
        {
            if (seeker != null)
                seeker.StartPath(rb.position, targetFinder.currentTarget.position, OnPathComplete);

            yield return new WaitForSeconds(pathUpdateInterval);
        }
    }

    private void MoveAlongPath()
    {
        if (path == null || currentWaypoint >= path.vectorPath.Count)
        {
            StopMovement();
            return;
        }

        Vector2 dir = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);
        SetAnimatorDirection(dir);

        if (Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
            currentWaypoint++;
    }

    private void StopMovement()
    {
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private void LookAtTarget(Transform target)
    {
        Vector2 dir = (target.position - transform.position).normalized;
        SetAnimatorDirection(dir);
    }

    private void PerformAttack(Transform target)
    {
            //animator.SetTrigger("Attack");
    }

    private void SetAnimatorDirection(Vector2 dir)
    {
        if (!animator) return;

        animator.SetBool("Up", dir.y > 0.3f);
        animator.SetBool("Down", dir.y < -0.3f);
        animator.SetBool("Left", dir.x < -0.3f);
        animator.SetBool("Right", dir.x > 0.3f);
    }

    private void ResetFleeGoal()
    {
        if (fleeGoal != null)
            fleeGoal.SetActive(false);
    }

    private void CreateFleeGoal(Transform target)
    {
        if (fleeGoal == null)
            return;

        fleeGoal.SetActive(true);

        Vector2 fleeDir = ((Vector2)transform.position - (Vector2)target.position).normalized;
        Vector2 perp = new Vector2(-fleeDir.y, fleeDir.x);

        Vector2 offset = fleeDir * Random.Range(3f, 5f) + perp * Random.Range(-2f, 2f);
        fleeGoal.transform.position = (Vector2)transform.position + offset;

        if (seeker != null)
            seeker.StartPath(rb.position, fleeGoal.transform.position, OnPathComplete);
    }

    public void OnDamaged()
    {
        if (!isAggroed_internal)
            StartChasing();
    }

    public void OnDeath()
    {
        StopChasing();
        StopAllCoroutines();
    }
}
