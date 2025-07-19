using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
            int index = EnemySpawner.Instance.GetEnemies().IndexOf(collision.gameObject);
            if (index != -1) // IndexOf can return -1.
                EnemySpawner.Instance.DeleteEnemy(index);
        }
    }
}
