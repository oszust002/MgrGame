using System;
using UnityEngine;
using UnityEngine.UI;


public class Player : HealthEntity
{
    public Slider healthSlider;

    private void Start()
    {
        healthSlider.minValue = 0f;
        healthSlider.maxValue = health;
        healthSlider.value = health;
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