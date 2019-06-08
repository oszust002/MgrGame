using System;
using UnityEngine;
using UnityEngine.UI;


public class Player : HealthEntity
{
    public Slider healthSlider;
    [HideInInspector]
    public int maxHealth;

    private void Start()
    {
        healthSlider.minValue = 0f;
        SetMaxHealth(health);
    }

    public void SetMaxHealth(int newMaxHealth, bool resetHealth = true)
    {
        maxHealth = newMaxHealth;
        healthSlider.maxValue = maxHealth;
        if (resetHealth)
        {
            health = maxHealth;
            healthSlider.value = health;
        }
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        healthSlider.value = health;
    }

    public override void Die()
    {
        base.Die();
        GameManager.instance.PlayerDied();
    }
}