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

namespace Arashmup
{
    public class PlayerManager : MonoBehaviour
    {
        //PhotonView photonView;
        //PlayerController player;
        //GameUIManager uiManager;

        //void Start()
        //{
        //    photonView = GetComponent<PhotonView>();

        //    uiManager = FindObjectOfType<GameUIManager>();

        //    if (photonView.IsMine)
        //    {
        //        CreateController();
        //        uiManager.OnCountDownOver += OnCountDownOver;
        //        uiManager.StartCountDown();
        //    }
        //}

        //void OnCountDownOver()
        //{
        //    player.fireAllowed = true;
        //    uiManager.OnCountDownOver -= this.OnCountDownOver;
        //}

        void CreateController()
        {
            //player = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "PlayerController"), new Vector3(0, 0, -1), Quaternion.identity).GetComponent<PlayerController>();

            //CameraController camera = Instantiate(Resources.Load<GameObject>(Path.Combine("Prefabs", "PlayerCamera")), Vector3.zero, Quaternion.identity).GetComponent<CameraController>();
            //camera.Setup(player);

            //player.GetComponent<PlayerController>().followCamera = camera.GetComponent<Camera>();
            //player.moveAllowed = true;
        }
    }
}