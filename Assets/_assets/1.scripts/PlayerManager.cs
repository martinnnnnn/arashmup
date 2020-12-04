using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using Photon.Pun;
using System.IO;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    PhotonView photonView;
    PlayerController player;
    GameUIManager uiManager; 
    public ObjectSpawn weaponSpawner;

    void Start()
    {
        photonView = GetComponent<PhotonView>();

        uiManager = FindObjectOfType<GameUIManager>();
        weaponSpawner = FindObjectOfType<ObjectSpawn>();

        if (photonView.IsMine)
        {
            CreateController();
            uiManager.OnCountDownOver += OnCountDownOver;
            uiManager.StartCountDown();
        }
    }


    void Update()
    {
        if (PhotonNetwork.IsMasterClient && photonView.IsMine)
        {
            weaponSpawner.UpdateSpawn(photonView);
        }
    }

    [PunRPC]
    public void PRC_SpawnObject(string prefabName, Vector3 position)
    {
        GameObject spawnedObj = Instantiate(Resources.Load<GameObject>(Path.Combine("Prefabs", prefabName)), position, Quaternion.identity);
    }

    void OnCountDownOver()
    {
        player.fireAllowed = true;
        uiManager.OnCountDownOver -= this.OnCountDownOver;
    }

    void CreateController()
    {
        player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), new Vector3(0, 0, -1), Quaternion.identity).GetComponent<PlayerController>();

        string cameraPath = Path.Combine("Prefabs", "PlayerCamera");
        CameraController camera = Instantiate(Resources.Load<GameObject>(cameraPath), Vector3.zero, Quaternion.identity).GetComponent<CameraController>();
        camera.Setup(player);

        player.GetComponent<PlayerController>().followCamera = camera.GetComponent<Camera>();
        player.moveAllowed = true;
    }
}
