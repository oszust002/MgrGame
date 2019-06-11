using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Level
{
    public List<GameObject> enemies;
    public List<GameObject> specialEnemies;
    public float spawnRate;
    public float finishScore;
    public bool endTheGame;
    public int healthBonusReward;
}
