using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // Enemy attributes
    public int currentHealth;
    public int maxHealth = 100; // Health pool
    public int damage = 10; // Damage dealt to the tower
    public float attackRange = 2f; // Attack range
    public float attackSpeed = 1f; // Attacks per second 
    public int armorClass = 10; // Reduces damage via % (0-100), 10 AC = 10% reduction
    public float agility = 1f; // Movement speed modifier
    public int enemyValue; // Reward when enemy is killed

    protected NavMeshAgent agent; // Pathfinding agent
    protected GameObject currentTarget; // Closest tower target
    protected float nextAttackTime = 0f;
    
    //Health bar
    public GameObject healthBarPrefab; // Assign in the Inspector
    private HealthBar healthBar; // Reference to the health bar instance

    public string towerTag = "Tower";
    protected virtual void Start()
    {
        currentHealth = maxHealth;
        // Instantiate the health bar and set the target
        GameObject hbInstance = Instantiate(healthBarPrefab, transform.position, Quaternion.identity);
        healthBar = hbInstance.GetComponent<HealthBar>();
        healthBar.target = this.transform; // The health bar will follow this object
        healthBar.SetMaxHealth(maxHealth);
        
        // Set an offset to position the health bar above the object
        healthBar.offset = new Vector3(0, 2f, 0); // Adjust the height as needed
        
        // Adjust agent speed based on agility
        agent = GetComponent<NavMeshAgent>();
        agent.speed *= agility;
        
        FindNearestTower();
    }

    // Update enemy behavior
    protected virtual void Update()
    {
        // Check if the current target is still alive, if not, find a new one
        if (currentTarget == null || currentTarget.GetComponent<Tower>() == null)
        {
            FindNearestTower();
        }

        if (currentTarget != null)
        {
            // Move towards the tower
            agent.SetDestination(currentTarget.transform.position);

            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (distanceToTarget <= attackRange && Time.time >= nextAttackTime)
            {
                AttackTower();
                nextAttackTime = Time.time + (1f / attackSpeed); // Cooldown based on attack speed
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

        foreach (var towerObj in allTowers)
        {
            Tower tower = towerObj.GetComponent<Tower>();
            if (tower == null || tower.isPreview) continue; // Skip towers in preview mode

            float distance = Vector3.Distance(transform.position, towerObj.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestTower = towerObj;
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
    if (tower != null && !tower.isPreview) // Ensure tower is not in preview mode
    {
        tower.TakeDamage(damage);
    }
}
    
    
    // Calculate the effective damage after considering armor class
    private int CalculateDamage()
    {
        float damageReduction = armorClass / 100f;
        return Mathf.RoundToInt(damage * (1f - damageReduction));
    }

    // Take damage from a player/tower attack
    public virtual void TakeDamage(int amount)
    {
       // int effectiveDamage = Mathf.RoundToInt(amount * (1f - (armorClass / 100f))); // Apply damage reduction
       // currentHealth -= effectiveDamage;
       currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
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
        Destroy(healthBar.gameObject);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position,attackRange);
    }
}

