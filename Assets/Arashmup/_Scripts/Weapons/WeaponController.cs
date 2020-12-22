using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using Photon.Pun;
using System.IO;
using TMPro;
using DG.Tweening;
using System;

namespace Arashmup
{
    public class WeaponRef
    {
        Weapon weapon;
    }

    public class WeaponController : MonoBehaviour
    {
        [SerializeField] Weapon classic;
        [SerializeField] Weapon sniper;
        [SerializeField] Weapon bounce;
        [SerializeField] Weapon sticky;
        [SerializeField] Weapon fragmentation;

        public FloatVariable FireRate;

        PhotonView PV;
        Weapon equiped;

        public StringVariable AmmoLeftText;
        public GameEvent AmmoLeftTextChangedEvent;

        int ammoLeft;
        int AmmoLeft
        {
            get
            {
                return ammoLeft;
            }
            set
            {
                ammoLeft = value;
                if (PV.IsMine)
                {
                    if (ammoLeft >= 0)
                    {
                        AmmoLeftText.Value = ammoLeft.ToString();
                    }
                    else
                    {
                        AmmoLeftText.Value = "\u221E";
                    }
                    AmmoLeftTextChangedEvent.Raise();
                }
            }
        }

        void Awake()
        {
            PV = GetComponent<PhotonView>();

            classic.Init();
            sniper.Init();
            bounce.Init();
            sticky.Init();
            fragmentation.Init();

            Equip(Weapon.Type.Classic);
        }

        public void Fire(int actorNumber, int bulletID, Vector3 position, Vector2 direction, Collider2D[] ignoreColliders = null)
        {
            equiped.Fire(actorNumber, bulletID, position, direction, ignoreColliders);
            if (PV.IsMine)
            {
                if (equiped != classic)
                {
                    AmmoLeft--;
                    if (AmmoLeft <= 0)
                    {
                        Equip(Weapon.Type.Classic);
                    }
                }
            }
        }

        public void Equip(Weapon.Type newWeaponType)
        {
            PV.RPC(RPC_Functions.Equip, RpcTarget.All, newWeaponType);
            AmmoLeft = equiped.ammo;
            FireRate.SetValue(equiped.fireRate);
        }

        [PunRPC]
        public void RPC_Equip(Weapon.Type newWeaponType)
        {
            switch (newWeaponType)
            {
                case Weapon.Type.Classic:
                    equiped = classic;
                    break;

                case Weapon.Type.Sniper:
                    equiped = sniper;

                    break;
                case Weapon.Type.Bounce:
                    equiped = bounce;
                    break;

                case Weapon.Type.Sticky:
                    equiped = sticky;
                    break;
                case Weapon.Type.Fragmentation:
                    equiped = fragmentation;
                    break;
            }

        }

        public void OnGameOver()
        {
            ResetBulletPools();
        }

        public void ResetBulletPools()
        {
            classic.ResetBulletPools();
            sniper.ResetBulletPools();
            bounce.ResetBulletPools();
            sticky.ResetBulletPools();
            fragmentation.ResetBulletPools();
        }
    }
}