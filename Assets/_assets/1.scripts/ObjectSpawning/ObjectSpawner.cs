using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

namespace Arashmup
{
    public class ObjectSpawner : MonoBehaviour
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

        PhotonView photonView;

        List<int> spawnObjectsIndexes;

        List<SpawnPoint> spawnPoints;
        List<int> spawnPointsIndexes;

        float timeNextRespawn;
        float timeSinceSpawn;

        void Start()
        {
            photonView = GetComponent<PhotonView>();

            spawnPoints = GetComponentsInChildren<SpawnPoint>().ToList();

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
                    if (spawnPointsIndexes.Count == 0 || spawnObjectsIndexes.Count == 0) // no spawn points or spawn objets -> cannot spawn
                    {
                        return;
                    }

                    int spawnPointIndex = Random.Range(0, spawnPointsIndexes.Count);
                    SpawnPoint nextPoint = spawnPoints[spawnPointsIndexes[spawnPointIndex]];

                    // if an object is already on the spawn point, we skip the spawn. might change that later but this will avoid problems if all spawn points are taken
                    if (nextPoint.spawnedObject == null)
                    {
                        int objectTypeIndex = Random.Range(0, spawnObjectsIndexes.Count);
                        string prefabName = spawnableObjects[spawnObjectsIndexes[objectTypeIndex]].prefabName;

                        photonView.RPC("PRC_SpawnObject", RpcTarget.All, prefabName, spawnPointsIndexes[spawnPointIndex]);

                        spawnObjectsIndexes.RemoveAt(objectTypeIndex);
                        if (spawnObjectsIndexes.Count == 0)
                        {
                            AddSpawnObjectsIndexes();
                        }

                        spawnPointsIndexes.RemoveAt(spawnPointIndex);
                        if (spawnPointsIndexes.Count == 0)
                        {
                            AddSpawnPointsIndexes();
                        }
                    }

                    UpdateNextRespawn();
                }
            }

        }

        [PunRPC]
        void PRC_SpawnObject(string prefabName, int spawnPointIndex)
        {
            SpawnPoint spawnPoint = spawnPoints[spawnPointIndex];

            GameObject spawnedObj = Instantiate(Resources.Load<GameObject>(Path.Combine("Prefabs", prefabName)), spawnPoint.transform);
            spawnedObj.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, spawnPoint.transform.position.z - 1);

            SpawnObject spawned = spawnedObj.GetComponent<SpawnObject>();
            spawned.spawner = this;
            spawned.spawnPointIndex = spawnPointIndex;

            spawnPoint.spawnedObject = spawnedObj;
        }

        public void DestoySpawnedObject(SpawnObject spawnObject)
        {
            photonView.RPC(RPC_DestroyObject_Name, RpcTarget.All, spawnObject.spawnPointIndex);
        }

        static string RPC_DestroyObject_Name = "RPC_DestroyObject";
        [PunRPC]
        void RPC_DestroyObject(int spawnPointIndex)
        {
            Destroy(spawnPoints[spawnPointIndex].spawnedObject);
            spawnPoints[spawnPointIndex].spawnedObject = null;
        }
        void UpdateNextRespawn()
        {
            timeSinceSpawn = 0;
            timeNextRespawn = Random.Range(minRespawnTime, maxRespawnTime);
        }
    }
}