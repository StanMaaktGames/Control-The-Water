using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttacking : MonoBehaviour
{
    public UnityEngine.AI.NavMeshAgent agent;
    public GameObject player;

    Vector3 targetPosition;

    public float speed = 1f;
    public float runSpeed = 2.5f;
    public float rotationSpeed = 100; 
    public float defaultAttackCooldown = 5f;
    public float defaultRandomAttackCooldown = 0.25f;
    public float defaultAttackRange = 4f;
    public float defaultObserveRangeMin = 20;
    public float defaultObserveRangeMax = 30;
    public float defaultNextAttackChance = 50;
    public float defaultObserveCooldown = 10f;
    
    float distance;
    float attackCooldown;
    float observeCooldown;
    string fightState = "close in";

    Animator animator;
    EnemyStateController EnemyStateController;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        EnemyStateController = GetComponent<EnemyStateController>();
    }

    void Update()
    {
        distance = Vector3.Distance(transform.position, player.transform.position);
        animator.SetFloat("distanceToPlayer", distance);
        attackCooldown -= Time.deltaTime;
        observeCooldown -= Time.deltaTime;
    }

    public void Fighting()
    {
        if (fightState == "close in")
        {
            agent.isStopped = false;
            agent.speed = speed;
            animator.SetInteger("movingState", 1);
            if (distance < defaultObserveRangeMax)
            {
                observeCooldown = defaultObserveCooldown;
                fightState = "observe";
            }
            else
            {
                agent.SetDestination(PositionAtDistanceFromPlayer(defaultObserveRangeMax));
            }
        }
        else if (fightState == "observe")
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.speed = speed;
            RotateTowardsPlayer();
            animator.SetInteger("movingState", 0);
            if (distance < defaultObserveRangeMin || observeCooldown <= 0)
            {
                fightState = "attack";
            }
            else if (distance > defaultObserveRangeMax)
            {
                fightState = "close in";
                EnemyStateController.LoseInterest();
            }
        }
        else if (fightState == "attack")
        {
            RotateTowardsPlayer();
            if (attackCooldown <= 0 && distance <= defaultAttackRange)
            {
                agent.isStopped = true;
                agent.speed = runSpeed;
                animator.SetInteger("movingState", 2);
                animator.SetTrigger("attack1");
                attackCooldown = defaultAttackCooldown * Random.Range(1 - defaultRandomAttackCooldown, 1 + defaultRandomAttackCooldown);
                agent.ResetPath();
                if (defaultNextAttackChance < Random.Range(0, 100))
                {
                    fightState = "back up";
                }
            }
            if (attackCooldown > 0)
            {
                agent.speed = speed;
                agent.isStopped = true;
                animator.SetInteger("movingState", -1);
            }
            else if (distance > defaultAttackRange)
            {
                agent.isStopped = false;
                agent.speed = runSpeed;
                animator.SetInteger("movingState", 2);
                agent.SetDestination(player.transform.position); 
            }
            else
            {
                agent.isStopped = true;
                agent.speed = speed;
                animator.SetInteger("movingState", 0);
            }
        }
        else if (fightState == "back up")
        {
            RotateTowardsPlayer();
            agent.speed = speed;
            animator.SetInteger("movingState", -1);
            if (distance > defaultObserveRangeMin)
            {
                observeCooldown = defaultObserveCooldown;
                fightState = "observe";
            }
            else if (distance < defaultObserveRangeMin)
            {
                fightState = "attack";
            }
            else
            {
                agent.SetDestination(PositionAtDistanceFromPlayer(defaultObserveRangeMin + 1));
            }
        }
    }

    void RotateTowardsPlayer()
    {
        Vector3 direction = player.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
    }

    Vector3 PositionAtDistanceFromPlayer(float x)
    {
        Vector3 A = player.transform.position;
        Vector3 B = transform.position;
        targetPosition = x * Vector3.Normalize(B - A) + A;
        return targetPosition;
    }
}
