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
    public class BulletController_Sticky : Bullet
    {
        float speed;
        int damage;
        int actorNumber;

        Rigidbody2D rigidBody;
        bool hasStopped = false;
        Collider2D ownCollider;
        Collider2D[] ignoreColliders;

        public void Setup(int actorNumber, int bulletID, Vector3 position, Vector2 direction, float speed, int damage, Collider2D[] toIgnore = null)
        {
            this.actorNumber = actorNumber;
            ID = bulletID;
            transform.position = position;
            this.speed = speed;
            this.damage = damage;
            ignoreColliders = toIgnore;

            gameObject.layer = LayerMask.NameToLayer("Bullet");

            rigidBody = GetComponent<Rigidbody2D>();
            rigidBody.bodyType = RigidbodyType2D.Dynamic;
            rigidBody.velocity = direction * speed;
            ownCollider = GetComponent<Collider2D>();

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
            if (!hasStopped)
            {
                hasStopped = true;

                CharacterDamage character = collision.collider.GetComponent<CharacterDamage>();
                if (character != null)
                {
                    character.ReceiveDamage(actorNumber, this, damage);
                }
                else
                {
                    gameObject.layer = LayerMask.NameToLayer("Sticky");
                    rigidBody.bodyType = RigidbodyType2D.Kinematic;
                    rigidBody.velocity = Vector2.zero;

                    if (ignoreColliders != null)
                    {
                        foreach (Collider2D collider in ignoreColliders)
                        {
                            Physics2D.IgnoreCollision(collider, ownCollider, false);
                        }
                    }
                }
            }
        }
    }
}