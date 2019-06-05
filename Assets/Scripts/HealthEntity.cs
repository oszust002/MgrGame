using UnityEngine;


public class HealthEntity : MonoBehaviour
{

    public int health = 30;
    public GameObject deathEffect;
    public AudioClip deathSound;

    public virtual void Die()
    {
        if (deathEffect != null)
        {
            var instantiate = Instantiate(deathEffect, transform.position, transform.rotation);
            Destroy(instantiate, 3f);
        }

        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }
        Destroy(gameObject);
    }
    
    public virtual void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }
}