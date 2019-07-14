﻿using System.Collections;
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
        if (enemies == null)
        {
            enemies = new List<GameObject>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Progress.instance.IsLevelLoading || GameManager.instance.gameEnded || GameManager.instance.gamePaused)
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
            }
            m_NextSpawnTime = Time.time + spawnTime;
        }

        if (AffectiveManager.instance.AffectiveEnabled())
        {
            HandleEmotion();
        }
    }

    private void HandleEmotion()
    {
        var emotion = AffectiveManager.instance.emotionManager.GetEmotion();
        //TODO: Handle emotion (if bored or neutral spawn extra wave)
    }
}
