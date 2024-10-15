using System;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    protected Transform target;
    public int ammoDamage;
    public int difficulty = 35; 
    public float ammoSpeed;
    public float armorPenetration; // AP value for each ammo type
    public bool isHoming; // Determines if the projectile is homing
    public bool isArching; // Determines if the projectile follows an arc

    public void Track(Transform _target)
    {
        target = _target;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Handle different movement types
        if (isHoming)
        {
            MoveHoming();
        }
        else if (isArching)
        {
            MoveArching();
           // ammoDamage = ammoDamage
        }
        else
        {
            MoveStraight();
           // ammoDamage = ammoDamage * difficulty ;
        }
    }

    protected virtual void MoveHoming()
    {
        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = ammoSpeed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    protected virtual void MoveArching()
    {
        // Simulate an arcing projectile
        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = ammoSpeed * Time.deltaTime;
        dir.y += Mathf.Sin(Time.time * ammoSpeed) * 0.5f; // Add arching effect

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    protected virtual void MoveStraight()
    {
        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = ammoSpeed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    private void HitTarget()
    {
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            // Apply armor penetration and calculate actual damage
            int finalDamage = Mathf.FloorToInt(ammoDamage * (1 - enemy.armorClass / 100f * (1 - armorPenetration / 100f)));
            enemy.TakeDamage(finalDamage * difficulty);
        }
        Destroy(gameObject);
    }
}


    

