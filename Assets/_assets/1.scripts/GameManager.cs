using System.Collections;
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
        int deadPlayerCount = 0;

        public PlayerCharacterRuntimeSet PlayerCharacters;

        public SceneFlowData SceneFlow;

        public IntVariable PlayersAliveCount;

        [Header("Game Events")]
        public GameEvent GameInitialized;
        public GameEvent LocalPlayerWon;
        public GameEvent LocalPlayerDied;


        void Start()
        {
            SceneFlow.backFromGameplay = true;
            SceneFlow.stayInRoom = true;

            PlayersAliveCount.SetValue(PhotonNetwork.PlayerList.Count());

            CreateController();
            GameInitialized.Raise();
        }

        void CreateController()
        {
            PhotonNetwork.Instantiate(RuntimePrefabsPaths.PlayerCharacter, Vector3.zero, Quaternion.identity);
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
            
            PlayersAliveCount.SetValue(PhotonNetwork.PlayerList.Count() - deadPlayerCount);

            CheckEnd();
        }

        bool localDead = false;
        public void IncreaseDead(bool isLocal)
        {
            deadPlayerCount++;

            PlayersAliveCount.SetValue(PhotonNetwork.PlayerList.Count() - deadPlayerCount);

            if (isLocal && !localDead)
            {
                localDead = true;
                LocalPlayerDied.Raise();
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
                    LocalPlayerWon.Raise();
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