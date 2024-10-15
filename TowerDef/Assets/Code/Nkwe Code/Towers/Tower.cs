using System;
using Unity.VisualScripting;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public event TowerDeath OnDeath;
    public delegate void TowerDeath();

    public string enemyTag = "Enemy";
    private Transform target;
    public Transform partRotate;
    public Transform firePoint;
    public float rotateSpeed = 10f;

    // Health bar
    public GameObject healthBarPrefab;
    private HealthBar healthBar;

    [Header("Stats")]
    public float range;
    public int maxHealth;
    public int currentHealth = 100;
    public float attackSpeed = 1f;
    public float attackCooldown = 0f;
    public GameObject ammoType;
    public int towerCost;
    public int towerDmg;

    // Add this flag to indicate whether the tower is in preview mode
    public bool isPreview = false;

    public void TakeDamage(int damage)
    {
        if (isPreview) return; // Don't take damage if in preview mode

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Update the health bar UI
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
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
        Destroy(healthBar.gameObject);
    }

    private void Start()
    {
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


