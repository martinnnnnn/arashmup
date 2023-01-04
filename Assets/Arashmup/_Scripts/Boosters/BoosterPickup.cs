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
    public class BoosterPickup : SpawnObject
    {
        public Booster booster;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<PlayerCharacter>(out var player))
            {
                if (other.TryGetComponent<BoosterController>(out var boosterController))
                {
                    boosterController.Add(booster);
                }
                DestroySelf();
            }
        }
    }
}