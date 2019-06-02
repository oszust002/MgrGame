using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : HealthEntity
{

    public int collisionDamage = 92;
    // Start is called before the first frame update
    public SpriteRenderer spriteRenderer;
    private float initHealth;
    public int killReward = 5;

    private void Start()
    {
        initHealth = health;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(collisionDamage);
            Debug.Log("Enemy death"); //Death logic (animations etc)
            base.Die();
        }
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        var inverseLerp = Mathf.InverseLerp(0, initHealth, health);
        spriteRenderer.color = Color.Lerp(Color.red, Color.white, inverseLerp);
    }

    public override void Die()
    {
        Progress.instance.AddScore(killReward);
        base.Die();
    }

    public void Remove()
    {
        base.Die();
    }
}
