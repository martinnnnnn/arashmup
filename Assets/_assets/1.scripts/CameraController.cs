using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class CameraController : MonoBehaviour
{
    PlayerController playerController;

    public void Setup(PlayerController player)
    {
        playerController = player;
    }

    void FixedUpdate()
    {
        Vector3 newPosition = transform.position * 0.9f + playerController.transform.position * 0.1f;

        transform.position = new Vector3(newPosition.x, newPosition.y, -10);
    }
}
