using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using Photon.Pun;
using System.IO;
using TMPro;
using DG.Tweening;

using Hashtable = ExitGames.Client.Photon.Hashtable;


namespace Arashmup
{
    public class CharacterDamage : MonoBehaviour
    {
        public BoolReference IsDead;

        CharacterProxy proxy;
        BoosterController boosterController;

        void Start()
        {
            proxy = GetComponent<CharacterProxy>();
            boosterController = GetComponent<BoosterController>();
        }

        public void ReceiveDamage(int actorNumber, Bullet bullet, int damage)
        {
            if (!IsDead.Value)
            {
                proxy.KillBullet(bullet);

                if (boosterController.IsDamageDone(damage))
                {
                    IncreaseKillFeed(actorNumber);
                    proxy.Kill();
                }
            }
        }

        void IncreaseKillFeed(int actorNumber)
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Count(); ++i)
            {
                if (PhotonNetwork.PlayerList[i].ActorNumber == actorNumber)
                {
                    int killCount = 0;
                    if (PhotonNetwork.PlayerList[i].CustomProperties.ContainsKey(CustomPropertiesKeys.KillCount))
                    {
                        killCount = (int)PhotonNetwork.PlayerList[i].CustomProperties[CustomPropertiesKeys.KillCount];
                    }
                    Hashtable hash = new Hashtable();
                    hash.Add(CustomPropertiesKeys.KillCount, ++killCount);
                    PhotonNetwork.PlayerList[i].SetCustomProperties(hash);
                    break;
                }
            }
        }
    }
}