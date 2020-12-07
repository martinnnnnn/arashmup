using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class SpawnObject : MonoBehaviour
{
    [HideInInspector] public ObjectSpawner spawner;
    [HideInInspector] public int spawnPointIndex;

    public void DestroySelf()
    {
        Debug.Log("spawner: " + (spawner != null ? "good" : "null") + " this: " + (this != null ? "good" : "null"));
        spawner.DestoySpawnedObject(this);
    }
}
