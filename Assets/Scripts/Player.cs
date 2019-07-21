using System;
using UnityEngine;
using UnityEngine.UI;


public class Player : HealthEntity
{
    public Slider healthSlider;
    [HideInInspector] public int maxHealth;
    private PlayerShooter playerShooter;

    private void Start()
    {
        healthSlider.minValue = 0f;
        SetMaxHealth(health);
        if (AffectiveManager.instance.AffectiveEnabled())
        {
            AffectiveManager.instance.emotionManager.onNewEmotion += HandleEmotion;
        }

        playerShooter = GetComponent<PlayerShooter>();
    }

    private void HandleEmotion(Emotion emotion)
    {
        //TODO: Handle emotion (if feared then heal or sth), maybe take shooter and special power if feared
        if (Emotion.Scared.Equals(emotion) || Emotion.Sad.Equals(emotion))
        {
            Heal((int) (0.1*maxHealth));
        }
        else if (Emotion.Angry.Equals(emotion))
        {
            if (playerShooter != null) playerShooter.ResetSpecial();
        }
        else if (Emotion.Happy.Equals(emotion))
        {
            TakeDamage((int) (0.2*health));
        }
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

    public void Heal(int healthPoints)
    {
        health += healthPoints;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }
}