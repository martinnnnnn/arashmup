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
    public class BulletController_Fragmentation : Bullet
    {
        public GameObject fragmentation;

        int actorNumber;
        float speed;
        float fragmentationDelay;
        int fragmentationCount;
        int damage;

        float timeSinceSpawn;

        public void Setup(int actorNumber, Vector3 position, Vector2 direction, float speed, float fragmentationDelay, int fragmentationCount, int damage, Collider2D[] ignoreColliders = null)
        {
            this.actorNumber = actorNumber;
            transform.position = position;
            this.speed = speed;
            this.fragmentationDelay = fragmentationDelay;
            this.fragmentationCount = fragmentationCount;
            this.damage = damage;

            timeSinceSpawn = 0;

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

        void Update()
        {
            fragmentationCount = 10;
            timeSinceSpawn += Time.deltaTime;
            if (timeSinceSpawn >= fragmentationDelay)
            {
                for (float i = 0.0f; i < fragmentationCount; i++)
                {
                    Rigidbody2D debrisbody = Instantiate(fragmentation, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
                    debrisbody.transform.Rotate(new Vector3(0, 0, 1), (i / (float)fragmentationCount) * 360);
                    debrisbody.velocity = debrisbody.transform.right * 3;
                }
                gameObject.SetActive(false);
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            PlayerController player = collision.collider.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ReceiveDamage(actorNumber, damage);
            }
            else
            {

            }
            gameObject.SetActive(false);
        }
    }
}