using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System;
using UnityEngine.UI;

namespace Arashmup
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        [SerializeField] TMP_InputField playerNameField;

        [SerializeField] TMP_InputField roomNameInputField;
        [SerializeField] TMP_Text errorText;
        [SerializeField] TMP_Text roomNameText;

        [SerializeField] Transform roomListContent;
        [SerializeField] GameObject roomListItemPrefab;

        [SerializeField] Transform playerListContent;
        [SerializeField] GameObject playerListItemPrefab;

        [SerializeField] GameObject startGameButton;

        public StringVariable PlayerName;

        public SceneFlowData SceneFlow;

        #region Server and Lobby
        void Start()
        {
            PlayerName.Value = PlayerPrefs.GetString(PlayerPrefsNames.PlayerName);
            if (string.IsNullOrEmpty(PlayerName.Value))
            {
                PlayerName.Value = playerNameField.text;
            }
            else
            {
                playerNameField.text = PlayerName.Value;
            }


            MenuManager.Instance.OpenMenu(Menu.Type.Loading);

            if (SceneFlow.backFromGameplay)
            {
                if (SceneFlow.stayInRoom)
                {
                    MenuManager.Instance.OpenMenu(Menu.Type.Room);
                    UpdateRoomData();
                }
                else if (PhotonNetwork.CurrentRoom != null)
                {
                    LeaveRoom();
                }
                SceneFlow.Reset();
            }
            else
            {
                if (!PhotonNetwork.IsConnected)
                {
                    Debug.Log("connect to server");

                    MenuManager.Instance.OpenMenu(Menu.Type.Loading);
                    PhotonNetwork.ConnectUsingSettings();
                }
            }
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to Master");
            PhotonNetwork.JoinLobby();
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.SerializationRate = 20;
            PhotonNetwork.SendRate = 60;
        }

        public override void OnJoinedLobby()
        {
            MenuManager.Instance.OpenMenu(Menu.Type.Title);
            if (string.IsNullOrEmpty(PhotonNetwork.NickName))
            {
                PhotonNetwork.NickName = PlayerName.Value;
            }
        }

        public void OnValidateNewName()
        {
            if (!string.IsNullOrEmpty(PlayerName.Value))
            {
                PlayerName.Value = playerNameField.text;
                PhotonNetwork.NickName = PlayerName.Value;
                PlayerPrefs.SetString("PlayerName", PlayerName.Value);
            }
        }

        public void ChangePlayerCharacter(RuntimeAnimatorController animController)
        {
            PlayerPrefs.SetString(PlayerPrefsNames.PlayerCharacter, animController.name);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        #endregion

        #region Room Create

        public void CreateRoom()
        {
            if (string.IsNullOrEmpty(roomNameInputField.text))
            {
                return;
            }

            PhotonNetwork.CreateRoom(roomNameInputField.text);
            MenuManager.Instance.OpenMenu(Menu.Type.Loading);
        }

        #endregion

        #region Room List

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach (Transform t in roomListContent)
            {
                Destroy(t.gameObject);
            }

            foreach (RoomInfo info in roomList)
            {
                if (!info.RemovedFromList && info.IsOpen)
                {
                    Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().Setup(info);
                }
            }
        }

        #endregion

        #region Room

        public override void OnJoinedRoom()
        {
            MenuManager.Instance.OpenMenu(Menu.Type.Room);

            UpdateRoomData();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            AddPlayerToRoom(newPlayer);
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        }

        public void JoinRoom(string roomName)
        {
            PhotonNetwork.JoinRoom(roomName);
            MenuManager.Instance.OpenMenu(Menu.Type.Loading);
        }

        void UpdateRoomData()
        {
            roomNameText.text = PhotonNetwork.CurrentRoom.Name;

            foreach (Transform t in playerListContent)
            {
                Destroy(t.gameObject);
            }

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                AddPlayerToRoom(player);
            }

            startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        }

        void AddPlayerToRoom(Player player)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().Setup(player);
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            MenuManager.Instance.OpenMenu(Menu.Type.Loading);
        }

        public override void OnLeftRoom()
        {
            MenuManager.Instance.OpenMenu(Menu.Type.Title);
        }

        #endregion

        public void StartGame()
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(1);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            errorText.text = "Room Creation Failed : " + message;
            MenuManager.Instance.OpenMenu(Menu.Type.Error);
        }
    }
}


//if (!PhotonNetwork.IsConnected)
//{
//    PhotonNetwork.ConnectUsingSettings();
//}
//else if(PhotonNetwork.InRoom)
//{
//    MenuManager.Instance.OpenMenu(Menu.Type.Room);
//}
//else
//{
//    MenuManager.Instance.OpenMenu(Menu.Type.Title);
//}