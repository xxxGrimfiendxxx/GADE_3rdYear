using System;
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
    
    [Header("Stats")]
    public float range;
    public int health = 100;
    public float attackSPeed = 1f;
    public float attackCooldown = 0f;
    public GameObject ammoType;
    
    
    
    
    // Function to deal damage to the tower
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    // Function to handle the tower's death
    private void Die()
    {
        Debug.Log(name + " has been destroyed.");
        OnDeath?.Invoke();
        Destroy(gameObject);
    }

    private void Start()
    {
        InvokeRepeating("UpdateTarget", 0f,0.5f);
    }

    private void UpdateTarget()// updates the 
    {
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

        if (nearestEnemy != null && shortestDistance <=range)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null; 
        }
    }

    void Update()
    {
        if (target == null)
        {
            return;
        }
        TargetLockOn();
        /*if (attackCooldown <= 0f)
        {
            attackCooldown = 1f / attackSPeed;
        }
        attackCooldown -= Time.deltaTime;*/
    }

    private void Shoot()
    {
       GameObject ammoGO = (GameObject)Instantiate(ammoType, firePoint.position, firePoint.rotation);
       Ammo ammo = ammoGO.GetComponent<Ammo>();
       if (ammo != null)
       {
           ammo.Track(target);
       }
       else
       {
           return;
       }
    }
    private void TargetLockOn()
    {
        if (partRotate == null)
        {
            Debug.Log
                ("PartRotate not assigned!");
            return;
        }
        if (target == null)
        {
            Debug.Log("Target Not found");
            return;
        }
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 actualRot = Quaternion.Lerp(partRotate.rotation, lookRotation, Time.deltaTime* rotateSpeed).eulerAngles;
        partRotate.rotation = Quaternion.Euler(0f, actualRot.y,0f);
        if (attackCooldown <= 0f)
        {
            if (ammoType != null && firePoint != null)
            {
                Shoot();
                attackCooldown = 1f / attackSPeed;
            }
            else
            {
                Debug.LogError("FirePoint or AmmoType not assigned.");
            }
        }

        attackCooldown -= Time.deltaTime;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position,range);
    }
    
    
}

