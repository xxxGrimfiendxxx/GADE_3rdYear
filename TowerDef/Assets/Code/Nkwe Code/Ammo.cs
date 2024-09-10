using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    private Transform target;
    public int damage;
    public float ammoSpeed = 70f;
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
        
        if (GameObject.FindGameObjectsWithTag("Enemy")!= null)
        { 
            Debug.Log("Im shot");
            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else
        {
            return;
        }
    }
    
}
