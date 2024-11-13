using System;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public UpgradeData[] upgrades;
    

    public UpgradeManager upgradeManager;
    public int upgradeCost => upgrades[currentUpgradeLevel].cost;

    public int currentUpgradeLevel = 0;  
    

    // Alternatively, use getter methods:
    public int GetCurrentUpgradeLevel() => currentUpgradeLevel;
    public HealthBar GetHealthBar() => healthBar;


    public event TowerDeath OnDeath;
    public delegate void TowerDeath();

    public string enemyTag = "Enemy";
    private Transform target;
    public Transform partRotate;
    public Transform firePoint;
    public float rotateSpeed = 10f;

    // Health bar
    public GameObject healthBarPrefab;
    public HealthBar healthBar;

    [Header("Stats")]
    public float range;
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public float attackSpeed = 1f;
    public float attackCooldown = 0f;
    public GameObject ammoType;
    public int towerCost;
    public int towerDmg;

    // Add this flag to indicate whether the tower is in preview mode
    public bool isPreview = false;
    public void UpgradeTower()
    {
        upgradeManager?.UpgradeTower(this);// Upgrade logic handled by UpgradeManager
          // Null-safe call
        if (currentUpgradeLevel < 2)  // Ensure it doesn't exceed level 3 (0, 1, 2)
        {
            currentUpgradeLevel++;
            // Reset health when upgrading
            currentHealth = maxHealth;

            // You can also trigger other upgrade effects like damage or range here
            HealthBarManager healthBarManager = FindObjectOfType<HealthBarManager>();
            if (healthBarManager != null)
            {
                healthBarManager.AddHealthBar(this);  // Recreate the health bar with new sprite for upgraded tower
            }
        }
    }
    

    public void TakeDamage(int damage)
    {
        if (isPreview) return; // Don't take damage if in preview mode

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Update the health bar UI
        healthBar?.SetHealth(currentHealth);  // Null-safe call
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isPreview) return; // No death behavior if in preview mode

        Debug.Log(name + " has been destroyed.");
        OnDeath?.Invoke();
        Destroy(gameObject);
        Destroy(healthBar?.gameObject);  // Null-safe call
    }

    private void Start()
    {
        upgradeManager = FindObjectOfType<UpgradeManager>();  // Finds the UpgradeManager in the scene

        currentHealth = maxHealth;
        if (!isPreview) // Only instantiate the health bar if the tower is active
        {
            GameObject hbInstance = Instantiate(healthBarPrefab, transform.position, Quaternion.identity);
            healthBar = hbInstance.GetComponent<HealthBar>();
            healthBar.target = this.transform;
            healthBar.SetMaxHealth(maxHealth);
            healthBar.offset = new Vector3(0, 2f, 0);
        }
        InvokeRepeating("UpdateTarget", 0f, 1f);
    }

    void OnMouseDown()
    {
        upgradeManager?.ShowUpgradeButton(this);  // Show upgrade UI for this tower
    }

    private void UpdateTarget()
    {
        if (isPreview) return; // Don't update target if in preview mode

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (var enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    private void Update()
    {
        if (isPreview || target == null) return;

        TargetLockOn();
    }

    private void TargetLockOn()
    {
        if (isPreview || partRotate == null) return;

        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 actualRot = Quaternion.Lerp(partRotate.rotation, lookRotation, Time.deltaTime * rotateSpeed).eulerAngles;
        partRotate.rotation = Quaternion.Euler(0f, actualRot.y, 0f);

        if (attackCooldown <= 0f)
        {
            Shoot();
            attackCooldown = 1f / attackSpeed;
        }

        attackCooldown -= Time.deltaTime;
    }

    private void Shoot()
    {
        if (ammoType == null || firePoint == null) return; // Guard clause to avoid errors

        GameObject ammoGO = Instantiate(ammoType, firePoint.position, firePoint.rotation);
        Ammo ammo = ammoGO.GetComponent<Ammo>();
        if (ammo != null)
        {
            ammo.Track(target);
            ammo.ammoDamage = towerDmg;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!isPreview)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}
