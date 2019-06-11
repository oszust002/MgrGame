
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public float fireRate = 4f;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public AudioClip shotSound;
    
    private float m_NextShotTime;
    private AudioSource weaponAudio;

    private void Awake()
    {
        weaponAudio = gameObject.AddComponent<AudioSource>();
        weaponAudio.volume = 0.2f;
        weaponAudio.loop = false;
        weaponAudio.playOnAwake = false;
    }

    private void Start()
    {
        weaponAudio.clip = shotSound;
    }

    private void Update()
    {
        if (GameManager.gameEnded)
        {
            return;
        }
        if (Time.time > m_NextShotTime)
        {
            weaponAudio.Play();
            var bulletObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Destroy(bulletObject, 10f);
            m_NextShotTime = Time.time + 1f / fireRate;
        }
    }
}