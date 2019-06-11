using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBomber : MonoBehaviour
{
    public float explosionRadius = 2f;
    public float timeToDestroy = 2f;
    public int damage = 20;
    public float explosionForce = 5f;
    public Animator explosionAnimation;

    private Enemy m_Enemy;

    private Rigidbody2D m_rb;
    

    // Start is called before the first frame update
    void Start()
    {
        m_Enemy = GetComponent<Enemy>();
        m_rb = GetComponent<Rigidbody2D>();
    }

    public void StartBombing()
    {
        explosionAnimation.SetTrigger("Explode");
        StartCoroutine(ExplodeBomb());
    }

    private IEnumerator ExplodeBomb()
    {
        float timer = 0f;
        while(timer < timeToDestroy) {
            while(GameManager.gamePaused) {
                yield return null;
            }
 
            timer += Time.deltaTime;
            yield return null;
        }
        
        Collider2D[] overlapCircle = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var t in overlapCircle)
        {
            //Have to check if game is paused to not destroy/move everything
            while(GameManager.gamePaused) {
                yield return null;
            }
            var rb = t.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                var direction = (rb.position - m_rb.position).normalized;
                rb.AddForce(direction*explosionForce, ForceMode2D.Impulse);
            }

            var component = t.GetComponent<Player>();
            if (component != null)
            {
                component.TakeDamage(damage);
            }
            
            BulletPath bullet = t.GetComponent<BulletPath>();
            if (bullet != null)
            {
                bullet.Remove();
            }
        }
        Debug.Log("BOOM");
        GetComponent<Enemy>().Remove();
    }
}
