using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float spawnInterval;
    [SerializeField] private TMP_Text killValue;
    private List<GameObject> enemies;
    private int kills = 0;
    private int killMax = 999;

    public static EnemySpawner Instance { get; private set; }

    private void Awake() 
    {
        if (Instance == null) 
        {
            Instance = this;
            Initialize();
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    void Initialize()
    {
        enemies = new List<GameObject>();
        InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
    }

    void Update()
    {
        if (enemies.Count > 0 && enemies[0].transform.position.y <= -5.5f)
        {
            DeleteEnemy();
            // Note that the enemy closest to the bottom of the screen will always be enemies[0].
        }
    }

    void SpawnEnemy()
    {
        GameObject newEnemy = Instantiate(enemyPrefab);
        newEnemy.transform.position = new Vector3(Random.Range(-6f, 6f), 6f, 0f);
        enemies.Add(newEnemy);
    }

    public void DeleteEnemy(int position = 0) // Note the default parameter value.
    {
        Destroy(enemies[position]);
        enemies.RemoveAt(position);
        kills = Mathf.Clamp(kills += 1, 0, killMax);
        killValue.text = kills.ToString();
    }

    public List<GameObject> GetEnemies()
    { // Just a C++ style getter for the enemies List. Why not.
        return enemies;
    }

    public void SetKillCount(int count)
    {
        kills = Mathf.Clamp(count, 0, killMax);
        killValue.text = kills.ToString();
    }

    public int GetKillCount()
    {
        return kills;
    }

    // What? A pair of C++ like getter/setters. C'mon Alex, this is C#/Unity.
    // Okay, okay, here's how you can do it with a property:

    /* Define this with the member variables at the top.
    
    public int KillCount
    {
        get { return kills; }
        set
        {
            kills = Mathf.Clamp(value, 0, killMax);
            killValue.text = kills.ToString();
        }
    }

    // And then just access KillCount from your other scripts.
    */
}
