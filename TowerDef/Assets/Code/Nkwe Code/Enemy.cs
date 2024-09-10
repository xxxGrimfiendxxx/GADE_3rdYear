using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int health = 100;
    public int damage = 10; // Damage dealt to the tower
    public float attackRange = 2f; // Distance required to attack the tower
    public float attackCooldown = 2f; // Time between attacks
    public int enemyValue; // Reward when enemy is killed

    protected NavMeshAgent agent; // Pathfinding agent
    protected GameObject currentTarget; // Closest tower target
    protected float nextAttackTime = 0f;

    public string towerTag = "Tower";
    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        FindNearestTower();
    }

    protected virtual void Update()
    {
        if (currentTarget == null)
        {
            FindNearestTower();
        }
        else
        {
            // Move towards the tower
            agent.SetDestination(currentTarget.transform.position);

            // Check if the enemy can attack the tower
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (distanceToTarget <= attackRange && Time.time >= nextAttackTime)
            {
                AttackTower();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    // Find the nearest tower
    protected void FindNearestTower()
    {
        GameObject[] allTowers = GameObject.FindGameObjectsWithTag(towerTag);
        if (allTowers.Length == 0) return;

        float shortestDistance = Mathf.Infinity;
        GameObject nearestTower = null;

        foreach (var tower in allTowers)
        {
            float distance = Vector3.Distance(transform.position, tower.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestTower = tower;
            }
        }

        if (nearestTower != null)
        {
            currentTarget = nearestTower.gameObject;
        }
    }

    // Attack the target tower
    protected virtual void AttackTower()
    {
        Tower tower = currentTarget.GetComponent<Tower>();
        if (tower != null)
        {
            tower.TakeDamage(damage);
        }
    }

    // Take damage from a player/tower attack
    public virtual void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    // Reward player and destroy the enemy
    protected virtual void Die()
    {
        PlayerInfo playerInfo = FindObjectOfType<PlayerInfo>();
        if (playerInfo != null)
        {
            playerInfo.GainCurrencyKill(enemyValue);
        }
        Destroy(gameObject);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position,attackRange);
    }
}

