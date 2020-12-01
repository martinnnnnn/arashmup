using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class BulletController : MonoBehaviour
{
    PlayerController shooter;

    public void Setup(PlayerController shooter)
    {
        this.shooter = shooter;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.collider.GetComponent<PlayerController>();
        if (player != null)
        {
            if (player != shooter)
            {
                player.ReceiveDamage(10);
            }
        }
        gameObject.SetActive(false);
    }
}
