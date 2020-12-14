using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using TMPro;

namespace Arashmup
{
    public class RoomListItem : MonoBehaviour
    {
        [SerializeField] TMP_Text text;

        RoomInfo roomInfo;
        public void Setup(RoomInfo info)
        {
            roomInfo = info;
            text.text = roomInfo.Name;
        }

        public void OnClick()
        {
            FindObjectOfType<Launcher>().JoinRoom(roomInfo.Name);
        }
    }
}