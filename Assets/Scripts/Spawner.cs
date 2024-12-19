using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;  //what to spawn
    [SerializeField] int numToSpawn;            //how many to spawn
    [SerializeField] int timeBetweenSpawns;     //wait time between spawns
    [SerializeField] Transform[] spawnPos;      //array to hold spawn positions
    [SerializeField] bool onePerPos;            //if true, limits one object per position

    int spawnCount = 0;                         //keep of how many objects have spawned so far

    bool startSpawning;                         //gives the green light to start spawning
    bool isSpawning;                            //while true means the spawner is not done spawning
    bool[] objectSpawned;                       //true if that specific position has spawned the object once

    public int NumToSpawn => numToSpawn;     //Read-only getter
    public int SpawnCount => spawnCount;     //Read-only getter
    public bool IsSpawning => isSpawning;     //Read-only getter

    // Start is called before the first frame update
    void Start()
    {
        //sets the size to how ever many spawn positions are stored
        objectSpawned = new bool[spawnPos.Length];
    }

    // Update is called once per frame
    void Update()
    {
        //spawns without overlapping spawning of other objects
        if (startSpawning && (spawnCount < numToSpawn) && !isSpawning)
        {
            StartCoroutine(spawn());        //start spawning
        }
        else if(spawnCount == numToSpawn)       //when done spawning
        {
            GameManager.instance.orbsSpawned();     //tells game manager how many orbs are in the scene
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            startSpawning = true;       //making sure there is a player before spawning
        }
    }

    IEnumerator spawn()
    {
        //starts spawning phase
        isSpawning = true;
        //cooldown between spawns
        yield return new WaitForSeconds(timeBetweenSpawns);
        //get a random position to spawn at from the ones stored
        int spawnInt = Random.Range(0, spawnPos.Length);
        //condition used when spawning at a position only once
        if (objectSpawned[spawnInt] == false)
        {
            //instantiates the object at the random spawn position
            Instantiate(objectToSpawn, spawnPos[spawnInt].position, spawnPos[spawnInt].rotation);
            //if true only one object per position spawns (number to spawn should be less than the number of positions in this case)
            if(onePerPos)
                objectSpawned[spawnInt] = true;     //if true, means that position spawned an object
            //increment number of objects spawned
            spawnCount++;

        }
        //spawning phase is done
        isSpawning = false;
    }
}
