using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generateRandomEnemy : MonoBehaviour
{
    [SerializeField] public GameObject typeOfEnemy;
    //[SerializeField] private GameObject whoToChase;

    [Header("Randomize Enemy Positions")]
    [SerializeField] private float xMin = -20f;
    [SerializeField] private float xMax = 20f;
    [SerializeField] private float yMin = -20f;
    [SerializeField] private float yMax = 20f;
    [SerializeField] private float zMin = -20f;
    [SerializeField] private float zMax = 20f;


    [Header("How Many Enemy to Create")]
    [SerializeField][Range(1, 50)] private int maximumEnemyCount;

    [Header("How Many Second to Wait for Enemy to Spawn")]
    [SerializeField][Range(0.5f, 10.0f)] private float spawningRate;

    // Position ranges for randomization
  
    public bool randomizeRespawnPosition = true;
    private int enemyCount;

    public void Start()
    {
        StartCoroutine(Respawn());        
    }

    private IEnumerator Respawn()
    {
        while (enemyCount < maximumEnemyCount)
        {
            Vector3 respawnPoint;

            //example to randomized the range of positions
            //xPos = Random.Range(-3, 34);
            //zPos = Random.Range(-26, 17);
            float xPos = Random.Range(xMin, xMax);
            float yPos = Random.Range(yMin, yMax);
            float zPos = Random.Range(zMin, zMax);

            if (randomizeRespawnPosition)
            {
                 xPos = Random.Range(xMin, xMax);
                 yPos = Random.Range(yMin, yMax);
                 zPos = Random.Range(zMin, zMax);
                respawnPoint = new Vector3(xPos, yPos, zPos);
            }
            // Spawn an enemy
            GameObject newEnemy = Instantiate(typeOfEnemy, new Vector3(xPos, 1f, zPos), Quaternion.identity);

            // Instantiate(enemyPrefab, new Vector3(xPos, 1f, zPos), Quaternion.identity);
            yield return new WaitForSeconds(spawningRate); // This should wait

            enemyCount += 1;

        }
    }


}

