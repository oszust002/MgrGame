using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpGenerator : MonoBehaviour
{

    public List<GameObject> weaponPowerUps;
    public float spawnTimeStep = 4f;
    public float maxPowerUps = 4f;
    public float radius = 2f;
    private static int existingPowerUpsCount;
    private float m_NextPowerUp;
    
    private System.Random random;

    // Start is called before the first frame update
    void Start()
    {
        m_NextPowerUp = Time.time + spawnTimeStep;
        existingPowerUpsCount = FindObjectsOfType<PowerUp>().Length;
        random = new System.Random();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > m_NextPowerUp && existingPowerUpsCount < maxPowerUps && weaponPowerUps.Count != 0)
        {
            var weaponPowerUp = weaponPowerUps[random.Next(weaponPowerUps.Count)];
            var spawnPosition = PlayerController.Position;
            //player position +- random x/y such as x^2+y^2=radius^2
            spawnPosition += Random.insideUnitCircle * radius;
            Instantiate(weaponPowerUp, spawnPosition, Quaternion.identity);
            m_NextPowerUp = Time.time + spawnTimeStep;
            existingPowerUpsCount++;
        }
    }
    
    
}
