using UnityEngine;


public class HealthEntity : MonoBehaviour
{

    public int health = 30;

    public virtual void Die()
    {
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