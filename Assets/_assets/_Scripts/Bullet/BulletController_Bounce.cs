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
    public class BulletController_Bounce : Bullet
    {
        float speed;
        int damage;
        int bounceCount;
        int actorNumber;

        Vector2 direction;

        Collider2D ownCollider;
        Collider2D[] ignoreColliders;
        Rigidbody2D rigidBody;
        int bounceCountLeft;

        public void Setup(int actorNumber, int bulletID, Vector3 position, Vector2 direction, float speed, int damage, int bounceCount, Collider2D[] toIgnore = null)
        {
            this.actorNumber = actorNumber;
            ID = bulletID;
            transform.position = position;
            this.direction = direction;
            this.speed = speed;
            this.damage = damage;
            this.bounceCount = bounceCount;
            ignoreColliders = toIgnore;

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
            CharacterDamage character = collision.collider.GetComponent<CharacterDamage>();
            if (character != null)
            {
                character.ReceiveDamage(actorNumber, this, damage);
            }

            if (bounceCountLeft == bounceCount) // we remove sender immunity at first bounce
            {
                if (ignoreColliders != null)
                {
                    foreach (Collider2D collider in ignoreColliders)
                    {
                        Physics2D.IgnoreCollision(collider, ownCollider, false);
                    }
                }
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
}