﻿using System.Collections;
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

    Weapon equiped;

    void Awake()
    {
        ammoLeftText = FindObjectOfType<GameUIManager>().ammoLeft;

        classic.Init();
        sniper.Init();
        bounce.Init();

        Equip(Weapon.Type.Classic);
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
        }

        AmmoLeft = equiped.ammo;
    }

    public float GetFireRate()
    {
        return equiped.fireRate;
    }
}