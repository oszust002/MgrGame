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
        if (AffectiveManager.instance.AffectiveEnabled())
        {
            AffectiveManager.instance.emotionManager.onNewEmotion += HandleEmotion;
        }
    }

    private void HandleEmotion(Emotion emotion)
    {
        //TODO: Handle emotion (if feared then heal or sth), maybe take shooter and special power if feared
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


    private void OnDestroy()
    {
        AffectiveManager.instance.emotionManager.onNewEmotion -= HandleEmotion;
    }
}