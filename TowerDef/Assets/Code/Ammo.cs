using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform target;
    public float ammoSpeed = 70f;
    public void Track(Transform _target)
    {
        target = _target;
    }

    // Update is called once per frame
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

    // ReSharper disable Unity.PerformanceAnalysis
    private void HitTarget()
    {
        Debug.Log("Im shot");
        if (GameObject.FindGameObjectsWithTag("Enemy")!= null)
        {
            Destroy(gameObject);
        }
        else
        {
            return;
        }
    }
    
}
