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


public class BoosterController : MonoBehaviour
{
    PlayerController playerController;
    PhotonView photonView;
    List<Booster> currentBoosters;

    void Start()
    {
        CustomTypesSerialization.Register();

        currentBoosters = new List<Booster>();
        playerController = GetComponent<PlayerController>();
        photonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        currentBoosters.RemoveAll(b =>
        {
            bool remove = false;

            if (b.useDuration)
            {
                b.durationLeft -= Time.deltaTime;

                if (b.durationLeft <= 0)
                {
                    remove = true;

                    switch (b.type)
                    {
                        case Booster.Type.Speed:
                            playerController.currentWalkSpeed = playerController.walkSpeed;
                            break;
                        case Booster.Type.Invincible:
                            break;
                        case Booster.Type.NoCooldownDash:

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

            switch (b.type)
            {
                case Booster.Type.Shield:
                    damageDone = false;
                    b.strength--;
                    if (b.strength <= 0)
                    {
                        remove = true;
                    }
                    break;

                case Booster.Type.Invincible:
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

        switch (booster.type)
        {
            case Booster.Type.Shield:
                break;
            case Booster.Type.Speed:
                playerController.currentWalkSpeed = booster.speed;
                break;
            case Booster.Type.Invincible:
                break;
            case Booster.Type.NoCooldownDash:
                playerController.currentDashRate = 0;
                break;
        }

        currentBoosters.Add(booster);
    }


}
