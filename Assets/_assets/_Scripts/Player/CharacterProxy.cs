﻿using Photon.Pun;
using System;
using UnityEngine;


namespace Arashmup
{
    public class CharacterProxy : MonoBehaviour
    {
        public BoolVariable IsDead;
        public SpriteRenderer Visual;
        public BulletRuntimeSet BulletsAlive;
        public GameEvent CharacterDeathEvent;

        PhotonView PV;
        WeaponController weaponController;
        Collider2D collider2d;

        int minBulletID;
        int maxBulletID;
        int currentBulletID;

        void Start()
        {
            PV = GetComponent<PhotonView>();
            weaponController = GetComponent<WeaponController>();
            collider2d = GetComponent<Collider2D>();

            IsDead.SetValue(false);

            minBulletID = PV.ViewID * 1000;
            maxBulletID = minBulletID + 999;
            currentBulletID = minBulletID;
        }


        public void Fire(int actorNumber, Vector3 position, Vector2 direction)
        {
            PV.RPC(RPC_Functions.Fire, RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, position, direction);
        }

        [PunRPC]
        void RPC_Fire(int actorNumber, Vector3 position, Vector2 direction)
        {
            Collider2D[] toIgnore = null;
            if (collider2d != null)
            {
                toIgnore = new Collider2D[]
                {
                    collider2d,
                };
            }

            weaponController.Fire(actorNumber, currentBulletID, position, direction, toIgnore);

            currentBulletID++;
            if (currentBulletID == maxBulletID)
            {
                currentBulletID = minBulletID;
            }
        }

        public void KillBullet(Bullet bullet)
        {
            PV.RPC(RPC_Functions.KillBullet, RpcTarget.All, bullet.ID);
        }

        [PunRPC]
        void RPC_KillBullet(int bulletID)
        {
            if(!BulletsAlive.Items.Exists(b => b.ID == bulletID))
            {
                Debug.LogError("This bullet does not exists");
                return;
            }

            Bullet localbullet = BulletsAlive.Items.Find(b => b.ID == bulletID);
            localbullet.gameObject.SetActive(false);
        }

        public void Kill()
        {
            PV.RPC(RPC_Functions.Kill, RpcTarget.All);
        }

        [PunRPC]
        void RPC_Kill()
        {
            IsDead.SetValue(true);
            Visual.color = new Color(Visual.color.r, Visual.color.g, Visual.color.b, 0.1f);
            CharacterDeathEvent.Raise();
        }
    }
}