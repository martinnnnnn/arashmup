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

public class GameManager : MonoBehaviourPunCallbacks
{
    [HideInInspector] public GameUIManager uiManager;

    //PhotonView photonView;

    int deadPlayerCount = 0;

    void Start()
    {
        //PhotonNetwork.AutomaticallySyncScene = false;
        uiManager = GetComponent<GameUIManager>();

        Debug.Log("hello");
        TransSceneData.Instance.backFromGameplay = true;
        TransSceneData.Instance.stayInRoom = true;
        //photonView = GetComponent<PhotonView>();
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateUI();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        //uiManager.stayInRoom.gameObject.SetActive(PhotonNetwork.IsMasterClient && gameOver);
    }

    public void UpdateUI(bool localDead = false)
    {
        deadPlayerCount++;

        // decrease alive
        int aliveCount = PhotonNetwork.PlayerList.Count() - deadPlayerCount;
        uiManager.aliveCount.text = aliveCount.ToString();

        // if mine 
            // show you loose
            // show leave
        uiManager.youDied.gameObject.SetActive(localDead);
        //uiManager.leaveRoom.gameObject.SetActive(localDead);

        // if over
            // if !lose
                // show win
                // show leave
            // if master
                // show back to room
        if (deadPlayerCount == PhotonNetwork.PlayerList.Count() - 1)
        {
            uiManager.youWon.gameObject.SetActive(!localDead);
            //uiManager.leaveRoom.gameObject.SetActive(true);
            //uiManager.stayInRoom.gameObject.SetActive(true);

            //uiManager.stayInRoom.gameObject.SetActive(PhotonNetwork.IsMasterClient);

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

    //public void IncreaseDead()
    //{
    //    deadPlayerCount++;
    //    EndGame();
    //}

    //public void EndGame()
    //{
    //    int newAliveCount = int.Parse(uiManager.aliveCount.text) - 1;
    //    uiManager.aliveCount.text = newAliveCount.ToString();

    //    if (deadPlayerCount == playerAliveCount - 1)
    //    {
    //        uiManager.stayInRoom.gameObject.SetActive(true);
    //        uiManager.leaveRoom.gameObject.SetActive(true); // making sure the button is visible for winner
    //    }
    //}

    //public void CheckWin()
    //{
    //    if (deadPlayerCount == playerAliveCount - 1)
    //    {
    //        uiManager.youWon.gameObject.SetActive(true);
    //        uiManager.stayInRoom.gameObject.SetActive(true);
    //        uiManager.leaveRoom.gameObject.SetActive(true); // making sure the button is visible for winner
    //    }
    //}

    //public void StayInRoom()
    //{
    //    //Hashtable hash = new Hashtable();
    //    //hash.Add("").
    //    //PhotonNetwork.CurrentRoom.SetCustomProperties(Hashtable propsToSet);
    //    //PhotonNetwork.AutomaticallySyncScene = false;
    //    TransSceneData.Instance.stayInRoom = true;
    //    LoadMenu();
    //}

    //public void LeaveRoom()
    //{
    //    LoadMenu();
    //}

    //public void LoadMenu()
    //{
    //    PhotonNetwork.CurrentRoom.IsOpen = true;
    //    TransSceneData.Instance.backFromGameplay = true;
    //    PhotonNetwork.LoadLevel(0);
    //    //SceneManager.LoadScene(0);
    //}
}
