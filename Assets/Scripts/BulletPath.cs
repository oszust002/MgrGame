using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPath : MonoBehaviour
{
    public float speed = 10f;
    public bool isEnemyBullet;
    public int damage = 10;
    public GameObject destroyEffect;

    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * speed;
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.gamePaused)
        {
            rb.velocity = Vector2.zero;
        }
        else
        {
            rb.velocity = transform.up * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var bullet = other.GetComponent<BulletPath>();
        if (bullet != null && !AreSameBulletType(bullet))
        {
            Explode();
            return;
        }
        
        if (isEnemyBullet)
        {
            var player = other.GetComponent<Player>();
            if (player == null) return;
            player.TakeDamage(damage);
            Explode();
        }
        else
        {
            var component = other.GetComponent<Enemy>();
            if (component == null) return;
            component.TakeDamage(damage);
            Explode();
        }
    }

    private bool AreSameBulletType(BulletPath bullet)
    {
        return bullet.isEnemyBullet == isEnemyBullet;
    }

    private void Explode()
    {
        var destroyEffectInstance = Instantiate(destroyEffect, transform.position, transform.rotation);
        Destroy(destroyEffectInstance, 1f);
        Destroy(gameObject);
    }

    // Update is called once per frame
    public void Remove()
    {
        Explode();
    }
}
