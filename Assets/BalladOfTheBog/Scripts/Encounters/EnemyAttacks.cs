using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Encounter/New Enemy Attacks")]
public class EnemyAttacks : ScriptableObject
{
    public Projectile projectile;
    public Vector2[] spawnPositions;
    public Vector2[] launchDirections;
    public float[] launchTimes;
    public float[] projectileSpeeds;
    public int endTime;
}
