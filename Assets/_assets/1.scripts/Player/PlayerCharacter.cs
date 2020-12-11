using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Arashmup
{
    public class PlayerCharacter : MonoBehaviour
    {
        public GameObject FollowCamera;
        public BoolReference IsDead;
        public StringVariable PlayerName;
        public GameEvent NameChangeEvent;
        public GameEvent LocalPlayerDied;

        PhotonView PV;
        CharacterProxy proxy;

        CharacterMovement movement;
        CharacterFire fire;
        CharacterDamage damage;
        Rigidbody2D rigidBody;
        Collider2D collider2d;


        void Awake()
        {
            PV = GetComponent<PhotonView>();
            proxy = GetComponent<CharacterProxy>();
            movement = GetComponent<CharacterMovement>();
            fire = GetComponent<CharacterFire>();
            damage = GetComponent<CharacterDamage>();
            rigidBody = GetComponent<Rigidbody2D>();
            collider2d = GetComponent<Collider2D>();

            if (PV.IsMine)
            {
                CameraController cameraController = Instantiate(FollowCamera, Vector3.zero, Quaternion.identity).GetComponent<CameraController>();
                fire.FollowCamera = cameraController;
            }
            else
            {
                Destroy(movement); movement = null;
                Destroy(movement); movement = null;
                Destroy(damage); damage = null;
                Destroy(rigidBody); rigidBody = null;
                Destroy(collider2d); collider2d = null;

                PlayerName = ScriptableObject.CreateInstance<StringVariable>();
                GetComponentInChildren<TextReplacer>().Variable = PlayerName;

                IsDead.Variable = ScriptableObject.CreateInstance<BoolVariable>();
                proxy.IsDead = IsDead.Variable;
            }

            PlayerName.Value = PV.Owner.NickName;
            NameChangeEvent.Raise();
        }

        public void OnDeath()
        {
            if (PV.IsMine)
            {
                LocalPlayerDied.Raise();
            }

            if (collider2d != null)
            {
                Destroy(collider2d);
            } 
        }

        #region Runtime Set
        public PlayerCharacterRuntimeSet RuntimeSet;

        private void OnEnable()
        {
            RuntimeSet.Add(this);
        }

        private void OnDisable()
        {
            RuntimeSet.Remove(this);
        }
        #endregion
    }
}