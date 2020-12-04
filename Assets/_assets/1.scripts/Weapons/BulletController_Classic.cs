using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class BulletController_Classic : MonoBehaviour
{
    float speed;
    int damage;
    public void Setup(Vector3 position, Vector2 direction, float speed, int damage, Collider2D[] ignoreColliders = null)
    {
        transform.position = position;
        this.speed = speed;
        this.damage = damage;

        GetComponent<Rigidbody2D>().velocity = direction * speed;
        Collider2D ownCollider = GetComponent<Collider2D>();

        if (ignoreColliders != null)
        {
            foreach (Collider2D collider in ignoreColliders)
            {
                Physics2D.IgnoreCollision(collider, ownCollider);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.collider.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ReceiveDamage(damage);
        }
        gameObject.SetActive(false);
    }
}
