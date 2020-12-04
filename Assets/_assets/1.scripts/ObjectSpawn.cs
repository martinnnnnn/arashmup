using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class ObjectSpawn : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnableObject
    {
        public string prefabName;
        [Range(0, 10)]
        public int frequency;
    }

    public SpawnableObject[] spawnableObjects;
    public float minRespawnTime;
    public float maxRespawnTime;

    List<int> spawnObjectsIndexes;

    List<Transform> spawnPoints;
    List<int> spawnPointsIndexes;

    float timeNextRespawn;
    float timeSinceSpawn;

    void Start()
    {
        spawnPoints = new List<Transform>();
        foreach (Transform t in transform)
        {
            spawnPoints.Add(t);
        }

        spawnPointsIndexes = new List<int>();
        AddSpawnPointsIndexes();

        spawnObjectsIndexes = new List<int>();
        AddSpawnObjectsIndexes();

        UpdateNextRespawn();
        timeSinceSpawn = 0;
    }

    void AddSpawnObjectsIndexes()
    {
        for (int i = 0; i < spawnableObjects.Length; ++i)
        {
            spawnObjectsIndexes.AddRange(Enumerable.Repeat(i, spawnableObjects[i].frequency));
        }
    }

    void AddSpawnPointsIndexes()
    {
        for (int i = 0; i < spawnPoints.Count; ++i)
        {
            spawnPointsIndexes.Add(i);
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            timeSinceSpawn += Time.deltaTime;
            if (timeSinceSpawn >= timeNextRespawn)
            {
                int spawnPointIndex = Random.Range(0, spawnPointsIndexes.Count);
                Transform nextPoint = spawnPoints[spawnPointIndex];
                spawnPointsIndexes.RemoveAt(spawnPointIndex);

                // spawn object
                int objectTypeIndex = Random.Range(0, spawnObjectsIndexes.Count);
                string prefabName = spawnableObjects[spawnObjectsIndexes[objectTypeIndex]].prefabName;
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", prefabName), nextPoint.position, Quaternion.identity);

                spawnObjectsIndexes.RemoveAt(objectTypeIndex);
                if (spawnObjectsIndexes.Count == 0)
                {
                    AddSpawnObjectsIndexes();
                }

                if (spawnPointsIndexes.Count == 0)
                {
                    AddSpawnPointsIndexes();
                }

                UpdateNextRespawn();
            }
        }
    }
    
    void UpdateNextRespawn()
    {
        timeSinceSpawn = 0;
        timeNextRespawn = Random.Range(minRespawnTime, maxRespawnTime);
    }
}
