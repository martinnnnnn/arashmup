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
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] Arashmup.Weapon classic;
        [SerializeField] Arashmup.Weapon sniper;
        [SerializeField] Arashmup.Weapon bounce;
        [SerializeField] Arashmup.Weapon sticky;
        [SerializeField] Arashmup.Weapon fragmentation;


        public FloatVariable FireRate;

        PhotonView photonView;
        Arashmup.Weapon equiped;

        public StringVariable AmmoLeftText;

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
                if (photonView.IsMine)
                {
                    if (ammoLeft >= 0)
                    {
                        AmmoLeftText.Value = ammoLeft.ToString();
                    }
                    else
                    {
                        AmmoLeftText.Value = "\u221E";
                    }
                }
            }
        }

        void Awake()
        {
            photonView = GetComponent<PhotonView>();

            classic.Init();
            sniper.Init();
            bounce.Init();
            sticky.Init();
            fragmentation.Init();

            Equip(Arashmup.Weapon.Type.Classic);
        }

        public void Fire(int actorNumber, Vector3 position, Vector2 direction, Collider2D[] ignoreColliders = null)
        {
            equiped.Fire(actorNumber, position, direction, ignoreColliders);
            if (equiped != classic)
            {
                AmmoLeft--;
                if (AmmoLeft <= 0)
                {
                    Equip(Arashmup.Weapon.Type.Classic);
                }
            }
        }

        public void Equip(Arashmup.Weapon.Type newWeaponType)
        {
            photonView.RPC(RPC_Functions.Equip, RpcTarget.All, newWeaponType);
        }

        [PunRPC]
        public void RPC_Equip(Arashmup.Weapon.Type newWeaponType)
        {
            switch (newWeaponType)
            {
                case Arashmup.Weapon.Type.Classic:
                    equiped = classic;
                    break;

                case Arashmup.Weapon.Type.Sniper:
                    equiped = sniper;

                    break;
                case Arashmup.Weapon.Type.Bounce:
                    equiped = bounce;
                    break;

                case Arashmup.Weapon.Type.Sticky:
                    equiped = sticky;
                    break;
                case Arashmup.Weapon.Type.Fragmentation:
                    equiped = fragmentation;
                    break;
            }

            AmmoLeft = equiped.ammo;
            FireRate.SetValue(equiped.fireRate);
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