using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peasant_ET : Enemy
{
    protected override void Start()
    {
        maxHealth = 50;
        currentHealth = maxHealth;
        agility = 1f;
        armorClass = 10; // Light armor
        damage = 20;
        attackSpeed = 1f;
        attackRange = 1f;
        enemyValue = 1; // Points for killing

        base.Start();
    }
}
