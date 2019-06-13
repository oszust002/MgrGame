using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpGenerator : MonoBehaviour
{

    public List<GameObject> weaponPowerUps;
    public float spawnTimeStep = 4f;
    public float maxPowerUps = 8f;
    public float radius = 2f;
    private static int _existingPowerUpsCount;
    private float m_NextPowerUp;
    
    private System.Random m_Random;

    // Start is called before the first frame update
    void Start()
    {
        m_NextPowerUp = Time.time + spawnTimeStep;
        _existingPowerUpsCount = FindObjectsOfType<PowerUp>().Length;
        m_Random = new System.Random();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Progress.instance.IsLevelLoading || GameManager.instance.gameEnded)
        {
            return;
        }
        
        if (Time.time > m_NextPowerUp && _existingPowerUpsCount < maxPowerUps && weaponPowerUps.Count != 0)
        {
            var weaponPowerUp = weaponPowerUps[m_Random.Next(weaponPowerUps.Count)];
            var spawnPosition = PlayerController.Position;
            //player position +- random x/y such as x^2+y^2=radius^2
            spawnPosition += Random.insideUnitCircle.normalized * radius;
            Instantiate(weaponPowerUp, spawnPosition, Quaternion.identity);
            m_NextPowerUp = Time.time + spawnTimeStep;
            _existingPowerUpsCount++;
        }
    }

    public static void RemovePowerUp()
    {
        _existingPowerUpsCount--;
        if (_existingPowerUpsCount < 0)
        {
            _existingPowerUpsCount = 0;
        }
    }


}
