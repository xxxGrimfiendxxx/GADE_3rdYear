using UnityEngine;

public partial class EnemySpeed : Enemy
{
    public float speedMultiplier = 2f;

    protected override void Start()
    {
        base.Start();
        agent.speed *= speedMultiplier; // Increase movement speed
        health = 70; // Lower health for fast enemy
    }
    
}

