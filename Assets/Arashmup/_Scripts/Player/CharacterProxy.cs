﻿using Photon.Pun;
using System;
using UnityEngine;


namespace Arashmup
{
    public class CharacterProxy : MonoBehaviour, IPunObservable
    {
        public BoolVariable IsDead;
        public Vector2Variable Direction;
        public BoolVariable IsDashing;
        public BulletRuntimeSet BulletsAlive;
        public GameEvent CharacterDeathEvent;
        public StringVariable AnimatorControllerName;
        public CharacterAnimation CharaAnimation;

        PhotonView PV;
        WeaponController weaponController;
        Collider2D collider2d;
        public SpriteRenderer visual;

        int minBulletID;
        int maxBulletID;
        int currentBulletID;

        // used for proxy character : local character sends Direction
        Vector2 directionRemote;

        // used for proxy character : local character sends Direction
        bool isDashingRemote;


        void Start()
        {
            PV = GetComponent<PhotonView>();
            weaponController = GetComponent<WeaponController>();
            collider2d = GetComponent<Collider2D>();
            visual = GetComponentInChildren<SpriteRenderer>();

            IsDead.SetValue(false);

            minBulletID = PV.ViewID * 1000;
            maxBulletID = minBulletID + 999;
            currentBulletID = minBulletID;
        }

        void Update()
        {
            if (invincibleColorEnabled)
            {
                float r = (Mathf.Sin((Time.timeSinceLevelLoad * 10.0f + (Mathf.PI * 2.0f * 1.0f / 3.0f))) + 1.0f) / 2.0f;
                float g = (Mathf.Sin((Time.timeSinceLevelLoad * 10.0f + (Mathf.PI * 2.0f * 2.0f / 3.0f))) + 1.0f) / 2.0f;
                float b = (Mathf.Sin((Time.timeSinceLevelLoad * 10.0f + (Mathf.PI * 2.0f * 3.0f / 3.0f))) + 1.0f) / 2.0f;

                visual.color = new Color(r, g, b);
            }

            HandleAnimation();
        }

        void HandleAnimation()
        {
            bool isDashing = false;
            Vector2 direction = Vector2.zero;

            if (PV.IsMine)
            {
                isDashing = IsDashing.Value;
                direction = Direction.Value;
            }
            else
            {
                isDashing = isDashingRemote;
                direction = directionRemote;
            }

            CharaAnimation.Animate(direction, isDashing);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(Direction.Value);
                stream.SendNext(IsDashing.Value);
            }
            else
            {
                directionRemote = (Vector2)stream.ReceiveNext();
                isDashingRemote = (bool)stream.ReceiveNext();
            }
        }

        public void Fire(int actorNumber, Vector3 position, Vector2 direction)
        {
            PV.RPC(RPC_Functions.Fire, RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, position, direction);
        }

        [PunRPC]
        void RPC_Fire(int actorNumber, Vector3 position, Vector2 direction)
        {
            Collider2D[] toIgnore = null;
            if (collider2d != null)
            {
                toIgnore = new Collider2D[]
                {
                    collider2d,
                };
            }

            weaponController.Fire(actorNumber, currentBulletID, position, direction, toIgnore);

            currentBulletID++;
            if (currentBulletID == maxBulletID)
            {
                currentBulletID = minBulletID;
            }
        }

        public void KillBullet(Bullet bullet)
        {
            PV.RPC(RPC_Functions.KillBullet, RpcTarget.All, bullet.ID);
        }

        [PunRPC]
        void RPC_KillBullet(int bulletID)
        {
            if(!BulletsAlive.Items.Exists(b => b.ID == bulletID))
            {
                Debug.LogWarning("This bullet does not exists");
                return;
            }

            Bullet localbullet = BulletsAlive.Items.Find(b => b.ID == bulletID);
            localbullet.gameObject.SetActive(false);
        }

        public void Kill()
        {
            PV.RPC(RPC_Functions.Kill, RpcTarget.All);
        }

        [PunRPC]
        void RPC_Kill()
        {
            IsDead.SetValue(true);
            visual.color = new Color(visual.color.r, visual.color.g, visual.color.b, 0.1f);
            CharacterDeathEvent.Raise();
        }

        bool invincibleColorEnabled = false;

        public void EnableInvincibleColor(bool enable)
        {
            PV.RPC(RPC_Functions.EnableInvincibleColor, RpcTarget.All, enable);
        }

        [PunRPC]
        public void RPC_EnableInvincibleColor(bool enable)
        {
            invincibleColorEnabled = enable;
            if (!invincibleColorEnabled)
            {
                visual.color = Color.white;
            }
        }

        public void SetCharacterAnimation()
        {
            PV = GetComponent<PhotonView>();
            PV.RPC(RPC_Functions.SetCharacterAnimation, RpcTarget.AllBuffered, AnimatorControllerName.Value);
        }

        [PunRPC]
        public void PRC_SetCharacterAnimation(string animatorName)
        {
            CharaAnimation.SetAnim(animatorName);
        }
    }
}

//Artist is Mezerg.
//If you're looking for another brilliant french electro artist using theremin you can try Spiky the Machinist, as well.