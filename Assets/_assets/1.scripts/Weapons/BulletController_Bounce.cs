using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class BulletController_Bounce : MonoBehaviour
{
    public float speed;
    public int damage;
    public int bounceCount;

    Vector2 direction;

    Collider2D ownCollider;
    Rigidbody2D rigidBody;
    int bounceCountLeft;

    public void Setup(Vector3 position, Vector2 dir, Collider2D[] ignoreColliders = null)
    {
        transform.position = position;
        direction = dir;

        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = direction * speed;

        ownCollider = GetComponent<Collider2D>();

        bounceCountLeft = bounceCount;

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
        if (bounceCountLeft > 0)
        {
            Vector2 newDirection = Vector2.Reflect(direction, collision.GetContact(0).normal);
            direction = newDirection;
            rigidBody.velocity = direction * speed;
            bounceCountLeft--;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
