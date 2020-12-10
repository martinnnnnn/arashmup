﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using DG.Tweening;

using Hashtable = ExitGames.Client.Photon.Hashtable;
/*
 * new weapons
 * shield
 * boosters
 * pad support
 * 
 * */


namespace Arashmup
{

    public class GameManager : MonoBehaviourPunCallbacks
    {
        [HideInInspector] public GameUIManager uiManager;

        int deadPlayerCount = 0;

        //PlayerCharacterRuntimeSet 
        public GameEvent GameInitialized;

        public SceneFlowData SceneFlow;

        void Start()
        {
            uiManager = GetComponent<GameUIManager>();

            SceneFlow.backFromGameplay = true;
            SceneFlow.stayInRoom = true;

            CreateController();
            GameInitialized.Raise();
        }

        void CreateController()
        {
            PhotonNetwork.Instantiate(RuntimePrefabsPaths.PlayerCharacter, Vector3.zero, Quaternion.identity);
            //player.moveAllowed = true;
        }

        public void OnCountdownOver()
        {
            //player.fireAllowed = true;
        }


        bool hasLeft = false;
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F10) && !hasLeft)
            {
                hasLeft = true;
                PhotonNetwork.LeaveRoom();
            }
        }

        public override void OnLeftRoom()
        {
            SceneFlow.backFromGameplay = true;
            SceneFlow.stayInRoom = false;
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            int aliveCount = PhotonNetwork.PlayerList.Count() - deadPlayerCount;
            uiManager.aliveCount.text = aliveCount.ToString();

            CheckEnd();
        }

        bool localDead = false;
        public void IncreaseDead(bool isLocal)
        {
            deadPlayerCount++;

            int aliveCount = PhotonNetwork.PlayerList.Count() - deadPlayerCount;
            uiManager.aliveCount.text = aliveCount.ToString();

            if (isLocal && !localDead)
            {
                localDead = true;
                uiManager.youDied.gameObject.SetActive(true);
            }
        }

        public bool CheckEnd()
        {
            if (deadPlayerCount >= PhotonNetwork.PlayerList.Count() - 1)
            {
                FindObjectsOfType<WeaponController>().ToList().ForEach(wc =>
                {
                    wc.ResetBulletPools();
                });

                if (!localDead)
                {
                    uiManager.youWon.gameObject.SetActive(true);
                    AddWinToLocalPlayer();
                }

                if (PhotonNetwork.IsMasterClient)
                {
                    StartCoroutine(EndGame());
                }
                return true;
            }
            return false;
        }

        IEnumerator EndGame()
        {
            yield return new WaitForSeconds(2);

            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.LoadLevel(0);
        }

        void AddWinToLocalPlayer()
        {
            int victoryCount = 0;
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(CustomPropertiesKeys.VictoryCount))
            {
                victoryCount = (int)PhotonNetwork.LocalPlayer.CustomProperties[CustomPropertiesKeys.VictoryCount];
            }
            Hashtable hash = new Hashtable();
            hash.Add(CustomPropertiesKeys.VictoryCount, ++victoryCount);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }
}