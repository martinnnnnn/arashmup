using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using TMPro;
using Photon.Pun;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    TMP_Text text;

    Player player;
    public void Setup(Player p)
    {
        text = GetComponent<TMP_Text>();
        player = p;
        text.text = player.NickName;
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
