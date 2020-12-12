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
        public PlayerCharacterRuntimeSet PlayerCharacters;
        public SceneFlowData SceneFlow;
        public IntVariable PlayersAliveCount;

        [Header("Game Events")]
        public GameEvent GameInitialized;
        public GameEvent GameOverEvent;
        public GameEvent CharacterDeathEvent;

        int deadPlayerCount = 0;


        void Start()
        {
            SceneFlow.backFromGameplay = true;
            SceneFlow.stayInRoom = true;

            PlayersAliveCount.SetValue(PhotonNetwork.PlayerList.Count());
            deadPlayerCount = 0;

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
            if (PlayerCharacters.Items.Exists(pc => pc.GetNetworkPlayer() == otherPlayer))
            {
                PlayerCharacter leaver = PlayerCharacters.Items.Find(pc => pc.GetNetworkPlayer() == otherPlayer);
                if (!leaver.IsDead.Value)
                {
                    CharacterDeathEvent.Raise();
                }
            }
        }

        public void OnCharacterDeath()
        {
            deadPlayerCount++;
            PlayersAliveCount.SetValue(PhotonNetwork.PlayerList.Count() - deadPlayerCount);
        }

        public void CheckEnd()
        {
            if (deadPlayerCount >= PhotonNetwork.PlayerList.Count() - 1)
            {
                GameOverEvent.Raise();

                if (PhotonNetwork.IsMasterClient)
                {
                    StartCoroutine(EndGame());
                }
            }
        }

        IEnumerator EndGame()
        {
            yield return new WaitForSeconds(2);

            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.LoadLevel(0);
        }


    }
}