using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

namespace Arashmup
{
    public class WeaponPickup : SpawnObject
    {
        public Arashmup.Weapon.Type weaponType;

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
}