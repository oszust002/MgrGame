using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> enemies;
    public float spawnRadius = 20f;
    public float spawnTime = 15f;
    
    private float m_NextSpawnTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Progress.instance.IsLevelLoading)
        {
            return;
        }
        
        if (Time.time > m_NextSpawnTime)
        {
            foreach (var enemy in enemies)
            {
                var spawnPosition = PlayerController.Position;
                //player position +- random x/y such as x^2+y^2=radius^2
                spawnPosition += Random.insideUnitCircle.normalized * spawnRadius;
                Instantiate(enemy, spawnPosition, Quaternion.identity);
                m_NextSpawnTime = Time.time + spawnTime;
            }
        }
    }
}
