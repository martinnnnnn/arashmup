using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class WeaponPickup : MonoBehaviour
{
    public Weapon.Type weaponType;


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hello");
        WeaponController weaponController = other.GetComponent<WeaponController>();
        if (weaponController != null)
        {
            weaponController.Equip(weaponType);
        }
        Destroy(gameObject);
    }
}
