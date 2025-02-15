using UnityEngine;

[CreateAssetMenu(menuName = "Encounter/New Enemy Attacks")]
public class EnemyAttacks : ScriptableObject
{
    // encounterScenery: 0 -> city, 1 -> bog
    public int encounterScenery;
    public Projectile projectile;
    public Vector2[] spawnPositions;
    public Vector2[] launchDirections;
    public float[] launchTimes;
    public float[] projectileSpeeds;
    public int endTime;
    public bool affectedByGravity;
}
