using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using TMPro;
using Photon.Pun;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    public TMP_Text playerName;
    public TMP_Text victoryCount;
    public TMP_Text killCount;

    Player player;
    public void Setup(Player p)
    {
        player = p;
        playerName.text = player.NickName;
        victoryCount.text = ((int)p.CustomProperties[CustomPropertiesKeys.VictoryCount]).ToString();
        killCount.text = ((int)p.CustomProperties[CustomPropertiesKeys.VictoryCount]).ToString();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer == player)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
