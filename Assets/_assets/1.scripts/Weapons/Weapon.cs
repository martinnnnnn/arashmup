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


[CreateAssetMenu(fileName = "Weapon", menuName = "MyAssets/Weapon")]
public class Weapon : ScriptableObject
{
    public enum Type
    {
        Classic,
        Sniper,
        Bounce,
        Sticky,
        Fragmentation,
    }

    [Header("Bullet")]
    public Type type;
    public GameObject bulletPrefab;
    public int ammo;
    public float fireRate;
    public float bulletSpeed;
    public int bulletDamage;
    public int bulletBounceCount;

    [Space(3)]
    [Header("Pool")]
    public GameObject poolPrefab;
    public int defaultPoolSize;

    ObjectPool bulletPool;

    public void Init()
    {
        bulletPool = Instantiate(poolPrefab, Vector3.zero, Quaternion.identity).GetComponent<ObjectPool>();
        bulletPool.Setup(bulletPrefab, defaultPoolSize);
    }

    public void Fire(int actorNumber, Vector3 position, Vector2 direction, Collider2D[] ignoreColliders = null)
    {
        GameObject bulletObj = bulletPool.GetNext();

        switch (type)
        {
            case Type.Classic:
                BulletController_Classic classic = bulletObj.GetComponent<BulletController_Classic>();
                classic.Setup(actorNumber, position, direction, bulletSpeed, bulletDamage, ignoreColliders);
                break;
            case Type.Sniper:
                BulletController_Sniper sniper = bulletObj.GetComponent<BulletController_Sniper>();
                sniper.Setup(actorNumber, position, direction, bulletSpeed, bulletDamage, ignoreColliders);
                break;
            case Type.Bounce:
                BulletController_Bounce bounce = bulletObj.GetComponent<BulletController_Bounce>();
                bounce.Setup(actorNumber, position, direction, bulletSpeed, bulletDamage, bulletBounceCount, ignoreColliders);
                break;
            case Type.Sticky:
                BulletController_Sticky sticky = bulletObj.GetComponent<BulletController_Sticky>();
                sticky.Setup(actorNumber, position, direction, bulletSpeed, bulletDamage, ignoreColliders);
                break;
            case Type.Fragmentation:
                BulletController_Fragmentation fragmentation = bulletObj.GetComponent<BulletController_Fragmentation>();
                fragmentation.Setup(actorNumber, position, direction, bulletSpeed, 1.0f, 5, bulletDamage, ignoreColliders);
                break;
        }
    }

    public void ResetBulletPools()
    {
        bulletPool.ResetAll();
    }
}