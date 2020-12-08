﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class WeaponPickup : SpawnObject
{
    public Weapon.Type weaponType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        WeaponController weaponController = other.GetComponent<WeaponController>();
        if (weaponController != null)
        {
            weaponController.Equip(weaponType);
            DestroySelf();
        }
    }
}
