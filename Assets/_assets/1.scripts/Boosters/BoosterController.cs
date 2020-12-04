using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using ExitGames.Client.Photon;




//[Serializable]
//public class Booster 
//{
//    public float duration;
//    [HideInInspector] public float durationLeft;
//}

//[Serializable]
//public class Booster_Shield : Booster
//{
//    public int count;
//}

//[Serializable]
//public class Booster_Invincible : Booster
//{
//}

//[Serializable]
//public class Booster_Speed : Booster
//{
//    public int value;
//}

//[Serializable]
//public class Booster_NoCooldownDash : Booster
//{
//}






public class BoosterController : MonoBehaviour
{
    PlayerController playerController;
    PhotonView photonView;
    List<Booster> currentBoosters;

    void Start()
    {
        ArashmupCustomTypes.Register();

        currentBoosters = new List<Booster>();
        playerController = GetComponent<PlayerController>();
        photonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        currentBoosters.RemoveAll(b =>
        {
            bool remove = false;

            if (b.duration > 0)
            {
                b.durationLeft -= Time.deltaTime;

                if (b.durationLeft <= 0)
                {
                    remove = true;

                    switch (b)
                    {
                        case Booster_Speed speed:
                            playerController.currentWalkSpeed = playerController.walkSpeed;
                            break;
                        case Booster_Invincible invincible:
                            break;
                        case Booster_NoCooldownDash dash:

                            playerController.currentDashRate = playerController.dashRate;
                            break;
                    }
                }
            }
            
            return remove;
        });
    }

    public bool IsDamageDone(int damage)
    {
        bool damageDone = true;

        currentBoosters.RemoveAll(b =>
        {
            bool remove = false;

            switch (b)
            {
                case Booster_Shield shield:
                    damageDone = false;
                    shield.count--;
                    if (shield.count <= 0)
                    {
                        remove = true;
                    }
                    break;

                case Booster_Invincible invincible:
                    damageDone = false;
                    break;
            }

            return remove;
        });

        return damageDone;
    }

    public void Add(Booster booster)
    {
        photonView.RPC("RPC_Add", RpcTarget.All, booster);
    }

    [PunRPC]
    void RPC_Add(Booster booster)
    {
        currentBoosters.RemoveAll(b => b.GetType() == booster.GetType());

        booster.durationLeft = booster.duration;

        switch (booster)
        {
            case Booster_Shield shield:
                break;
            case Booster_Speed speed:
                playerController.currentWalkSpeed = speed.value;
                speed.durationLeft = speed.value;
                break;
            case Booster_Invincible invincible:
                break;
            case Booster_NoCooldownDash dash:
                playerController.currentDashRate = 0;
                break;
        }

        currentBoosters.Add(booster);
    }


}
