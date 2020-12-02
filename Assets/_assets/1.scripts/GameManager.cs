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

    int deadPlayerCount = 0;

    void Start()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);

        uiManager = GetComponent<GameUIManager>();

        TransSceneData.Instance.backFromGameplay = true;
        TransSceneData.Instance.stayInRoom = true;
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        IncreaseDead(false);
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

    public void CheckEnd()
    {
        if (deadPlayerCount == PhotonNetwork.PlayerList.Count() - 1)
        {
            if (!localDead)
            {
                uiManager.youWon.gameObject.SetActive(true);
            }

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
