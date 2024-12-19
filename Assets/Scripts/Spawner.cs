using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int numToSpawn;
    [SerializeField] int timeBetweenSpawns;
    [SerializeField] Transform[] spawnPos;

    int spawnCount = 0;

    bool startSpawning;
    bool isSpawning;
    bool[] objectSpawned;

    public int NumToSpawn => numToSpawn;     //Read-only getter
    public int SpawnCount => spawnCount;     //Read-only getter
    public bool IsSpawning => isSpawning;     //Read-only getter

    // Start is called before the first frame update
    void Start()
    {
        //GameManager.instance.UpdateOrbsCollected(numToSpawn);

        objectSpawned = new bool[spawnPos.Length];
    }

    // Update is called once per frame
    void Update()
    {
        if (startSpawning && (spawnCount < numToSpawn) && !isSpawning)
        {
            StartCoroutine(spawn());
        }
        else if(spawnCount == numToSpawn)
        {
            GameManager.instance.orbsSpawned();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }

    IEnumerator spawn()
    {
        isSpawning = true;

        yield return new WaitForSeconds(timeBetweenSpawns);

        int spawnInt = Random.Range(0, spawnPos.Length);

        if (objectSpawned[spawnInt] == false)
        {
            Instantiate(objectToSpawn, spawnPos[spawnInt].position, spawnPos[spawnInt].rotation);

            objectSpawned[spawnInt] = true;

            spawnCount++;
        }

        isSpawning = false;
    }
}
