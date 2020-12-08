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

        victoryCount.text = "0";
        if (player.CustomProperties.ContainsKey(CustomPropertiesKeys.VictoryCount))
        {
            victoryCount.text = ((int)player.CustomProperties[CustomPropertiesKeys.VictoryCount]).ToString();
        }

        killCount.text = "0";
        if (player.CustomProperties.ContainsKey(CustomPropertiesKeys.KillCount))
        {
            killCount.text = ((int)player.CustomProperties[CustomPropertiesKeys.KillCount]).ToString();
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer == player)
        {
            victoryCount.text = "0";
            if (player.CustomProperties.ContainsKey(CustomPropertiesKeys.VictoryCount))
            {
                victoryCount.text = ((int)player.CustomProperties[CustomPropertiesKeys.VictoryCount]).ToString();
            }

            killCount.text = "0";
            if (player.CustomProperties.ContainsKey(CustomPropertiesKeys.KillCount))
            {
                killCount.text = ((int)player.CustomProperties[CustomPropertiesKeys.KillCount]).ToString();
            }
        }
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
//OnPlayerPropertiesUpdate