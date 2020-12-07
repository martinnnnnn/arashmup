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

public class WeaponController : MonoBehaviour
{
    [SerializeField] Weapon classic;
    [SerializeField] Weapon sniper;
    [SerializeField] Weapon bounce;
    [SerializeField] Weapon sticky;

    PhotonView photonView;
    TMP_Text ammoLeftText;
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
                    ammoLeftText.text = ammoLeft.ToString();
                }
                else
                {
                    ammoLeftText.text = "\u221E";
                }
            }
        }
    }

    Weapon equiped;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();

        ammoLeftText = FindObjectOfType<GameUIManager>().ammoLeft;

        classic.Init();
        sniper.Init();
        bounce.Init();
        sticky.Init();

        Equip(Weapon.Type.Sticky);
    }

    public void Fire(Vector3 position, Vector2 direction, Collider2D[] ignoreColliders = null)
    {
        equiped.Fire(position, direction, ignoreColliders);
        if (equiped != classic)
        {
            AmmoLeft--;
            if (AmmoLeft <= 0)
            {
                Equip(Weapon.Type.Classic);
            }
        }
    }

    public void Equip(Weapon.Type newWeaponType)
    {
        photonView.RPC("RPC_Equip", RpcTarget.All, newWeaponType);
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
        }

        AmmoLeft = equiped.ammo;
    }

    public float GetFireRate()
    {
        return equiped.fireRate;
    }
}