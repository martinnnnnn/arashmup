using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class BoosterPickup : SpawnObject
{
    public Booster booster;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("hello");

        BoosterController boosterController = other.GetComponent<BoosterController>();
        if (boosterController != null)
        {
            boosterController.Add(booster);
            DestroySelf();
        }
    }
}
