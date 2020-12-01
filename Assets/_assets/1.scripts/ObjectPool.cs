﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class ObjectPool : MonoBehaviour
{
    GameObject prefab;
    string prefabNetworkPath;
    int amount;
    public int growAmount = 10;

    List<GameObject> pool = new List<GameObject>();

    public void Setup(GameObject prefab, int amount)
    {
        this.prefab = prefab;

        Grow(amount);
    }

    void Grow(int amount)
    {
        this.amount += amount;

        for (int i = 0; i < amount; ++i)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);

            pool.Add(obj);
        }
    }

    public void SetupNetworked(string path, int amount)
    {
        prefabNetworkPath = path;

        GrowNetworked(amount);
    }
    
    void GrowNetworked(int amount)
    {
        this.amount += amount;

        for (int i = 0; i < amount; ++i)
        {
            GameObject obj = PhotonNetwork.Instantiate(prefabNetworkPath, Vector3.zero, Quaternion.identity);
            obj.transform.parent = transform;
            obj.SetActive(false);

            pool.Add(obj);
        }
    }

    public GameObject GetNext()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        Debug.LogWarning("No more free objects : growing pool");

        int nextFree = amount;
        Grow(growAmount);

        pool[nextFree].SetActive(true);

        return pool[nextFree];
    }
}