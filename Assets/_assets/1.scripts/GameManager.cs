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

public class GameManager : MonoBehaviourPunCallbacks
{
    [HideInInspector] public GameUIManager uiManager;

    int deadPlayerCount = 0;
    int localPlayerKills;

    void Start()
    {
        PhotonNetwork.Instantiate(Path.Combine("Prefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);

        uiManager = GetComponent<GameUIManager>();

        TransSceneData.Instance.backFromGameplay = true;
        TransSceneData.Instance.stayInRoom = true;
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
        TransSceneData.Instance.backFromGameplay = true;
        TransSceneData.Instance.stayInRoom = false;
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
        int victoryCount = (int)PhotonNetwork.LocalPlayer.CustomProperties[CustomPropertiesKeys.VictoryCount];

        Hashtable hash = new Hashtable();
        hash.Add(CustomPropertiesKeys.VictoryCount, victoryCount);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }
}