using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
    [SerializeField] float range;
    [SerializeField] float cooldown;
    [SerializeField] float bulletSpeed;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform spawnPoint;
    private GameObject target;
    private bool ready2Fire = true;
    // Start is called before the first frame update
    void Start()
    {
        target = null;
    }

    // Update is called once per frame
    void Update()
    {
        FindClosestEnemy();
        if (target != null)
        {
            transform.LookAt(target.transform.position);
            if (ready2Fire)
            {
                // Instantiate the bullet at the spawn point.
                GameObject bulletInst = GameObject.Instantiate(bulletPrefab, spawnPoint.position, Quaternion.identity);

                // Calculate the direction from the turret to the target.
                Vector3 directionToTarget = Vector3.Normalize((target.transform.position + new Vector3(0f, -0.5f, 0f)) - transform.TransformPoint(spawnPoint.localPosition));

                bulletInst.GetComponent<Rigidbody2D>().AddForce(directionToTarget * bulletSpeed, ForceMode2D.Impulse);
                Destroy(bulletInst, 3f);

                // Set ready2Fire to false and invoke the cooldown.
                ready2Fire = false;
                Invoke("FireCooldown", cooldown);
            }
        }
    }

    private void FindClosestEnemy()
    {
        target = null;
        float dist2Target;
        if (EnemySpawner.Instance.GetEnemies().Count > 0)
        {
            List<GameObject> tempEnemies = EnemySpawner.Instance.GetEnemies();
            for (int i = 0; i < tempEnemies.Count; i++)
            {
                float dist2NewEnemy = Vector3.Distance(transform.position, tempEnemies[i].transform.position);
                dist2Target = (target != null?Vector3.Distance(transform.position, target.transform.position):range);
                if (dist2NewEnemy < range && dist2NewEnemy < dist2Target)
                {
                    target = tempEnemies[i];
                    dist2Target = Vector3.Distance(transform.position, target.transform.position);
                }
            }
        }
    }

    private void FireCooldown()
    {
        ready2Fire = true;
    }
}
