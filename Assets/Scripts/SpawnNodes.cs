using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNodes : MonoBehaviour
{
    //int numToSpawn = 25;
    int numToSpawn = 28;
    public float currentSpawnOffset;
    public float spawnOffset = 0.299f;
    // Start is called before the first frame update
    void Start()
    {
        if(gameObject.name == "Node")
        {
            // gameObject.name = "Node";
            // return;
            currentSpawnOffset = spawnOffset;
            for(int i = 0; i < numToSpawn; i++)
            {
                GameObject clone = Instantiate(gameObject, new Vector3(transform.position.x , transform.position.y +currentSpawnOffset, 0), Quaternion.identity);
                currentSpawnOffset += spawnOffset;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}