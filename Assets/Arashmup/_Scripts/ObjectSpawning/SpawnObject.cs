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
    public class SpawnObject : MonoBehaviour
    {
        [HideInInspector] public ObjectSpawner spawner;
        [HideInInspector] public int spawnPointIndex;

        public void DestroySelf()
        {
            spawner.DestoySpawnedObject(this);
        }
    }
}