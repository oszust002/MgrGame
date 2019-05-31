
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public float fireRate = 4f;
    public Transform firePoint;
    public GameObject bulletPrefab;
    
    private float m_NextShotTime;

    private void Update()
    {
        if (Time.time > m_NextShotTime)
        {
            var bulletObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Destroy(bulletObject, 10f);
            m_NextShotTime = Time.time + 1f / fireRate;
        }
    }
}